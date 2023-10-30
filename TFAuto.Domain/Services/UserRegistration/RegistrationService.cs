using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Services.Authentication;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.UserPassword.DTO;
using TFAuto.Domain.Services.UserRegistration.Models.Request;
using TFAuto.Domain.Services.UserRegistration.Models.Response;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain;

public class RegistrationService : IRegistrationService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly IMemoryCache _memoryCache;
    private readonly IEmailService _emailService;
    private readonly JWTService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public RegistrationService(
        IRepository<User> repositoryUser,
        IConfiguration configuration,
        IMemoryCache memoryCache,
        JWTService jwtService,
        IEmailService emailService,
        IMapper mapper)
    {
        _repositoryUser = repositoryUser;
        _memoryCache = memoryCache;
        _emailService = emailService;
        _jwtService = jwtService;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async ValueTask<ConfirmRegistrationResponse> ConfirmEmailAsync(ConfirmRegistrationRequest userRequest, string baseUrl)
    {
        if (string.IsNullOrEmpty(baseUrl))
            throw new Exception("URL is missing in the request.");

        var userExistsByEmail = await _repositoryUser.GetAsync(c => c.Email == userRequest.Email).FirstOrDefaultAsync();
        if (userExistsByEmail != null)
            throw new ValidationException(ErrorMessages.USER_EXISTS_BY_EMAIL);

        User user = _mapper.Map<User>(userRequest);
        user.RoleId = RoleId.USER;

        var tokenSettings = GetPasswordResetSettings();

        var confirmToken = GenerateResetToken(tokenSettings.TokenLength);
        var сodeExpiration = DateTime.UtcNow.AddSeconds(tokenSettings.TokenLifetimeInSeconds);

        _memoryCache.Set(confirmToken, new TokenInfo { UserId = user.Id, Expiration = сodeExpiration }, сodeExpiration);
        _memoryCache.Set(user.Id, user, сodeExpiration);

        var resetLink = GenerateConfirmEmailLink(baseUrl, user.Id, confirmToken);
        await _emailService.SendConfirmationEmailAsync(userRequest.Email, resetLink);

        return new ConfirmRegistrationResponse { Message = "Email with further instructions has been successfully sent." };
    }

    public async ValueTask<RegistrationResponse> RegisterUserAsync(RegistrationRequest confirmEmailRequest)
    {
        var tokenInfo = _memoryCache.Get<TokenInfo>(confirmEmailRequest.Token);

        if (tokenInfo == null || tokenInfo.Expiration < DateTime.UtcNow)
            throw new ValidationException(ErrorMessages.INVALID_CONFIRM_TOKEN);

        var user = _memoryCache.Get<User>(tokenInfo.UserId);

        if (user == null)
            throw new ValidationException(ErrorMessages.USER_INVALID_INFO);

        User dataUser = await _repositoryUser.CreateAsync(user);

        _memoryCache.Remove(confirmEmailRequest.Token);
        _memoryCache.Remove(tokenInfo.UserId);

        UserRegistrationResponse responseUser = _mapper.Map<UserRegistrationResponse>(dataUser);

        var jwtToken = await _jwtService.GenerateTokenMode(user.Id, user.Email);

        var response = new RegistrationResponse
        {
            User = responseUser,
            Tokens = jwtToken
        };

        return response;
    }

    private TokenSettings GetPasswordResetSettings()
    {
        return _configuration.GetSection("TokenSettings").Get<TokenSettings>();
    }

    private static string GenerateConfirmEmailLink(string baseUrl, string id, string resetToken)
    {
        var resetLink = $"{baseUrl}register-confirm?id={id}&token={resetToken}";
        return resetLink;
    }

    private static string GenerateResetToken(int tokenLength)
    {
        if (tokenLength < 1)
            throw new ArgumentOutOfRangeException(nameof(tokenLength));

        Random rng = new Random();
        int minValue = (int)Math.Pow(10, tokenLength - 1);
        int maxValue = (int)Math.Pow(10, tokenLength) - 1;
        int tokenValue = rng.Next(minValue, maxValue + 1);

        var refreshToken = tokenValue.ToString().PadLeft(tokenLength, '0');

        return refreshToken;
    }
}
