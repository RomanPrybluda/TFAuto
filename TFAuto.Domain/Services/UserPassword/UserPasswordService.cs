using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.UserPassword.DTO;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.UserPassword
{
    public class UserPasswordService : IUserPasswordService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserPasswordService(
            IRepository<User> userRepository,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async ValueTask<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception("URL is missing in the request.");

            var user = await _userRepository.GetAsync(t => t.Email == request.Email).FirstOrDefaultAsync();

            if (user == null)
                throw new ValidationException(ErrorMessages.INVALID_EMAIL);

            var passResetSettings = GetPasswordResetSettings();

            var resetToken = GenerateResetToken(passResetSettings.TokenLength);
            var сodeExpiration = DateTime.UtcNow.AddSeconds(passResetSettings.TokenLifetimeInSeconds);

            _memoryCache.Set(resetToken, new TokenInfo { UserId = user.Id, Expiration = сodeExpiration }, сodeExpiration);

            var resetLink = GenerateRecoveryPasswordLink(baseUrl);
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetToken, resetLink);

            var response = new ForgotPasswordResponse { Message = "Email with further instructions has been successfully sent." };
            return response;
        }

        public async ValueTask<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenInfo = _memoryCache.Get<TokenInfo>(request.Token);

            if (tokenInfo == null || tokenInfo.Expiration < DateTime.UtcNow)
                throw new ValidationException(ErrorMessages.INVALID_TOKEN);

            var user = await _userRepository.TryGetAsync(tokenInfo.UserId, nameof(User));

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(request.Token);

            var response = new ResetPasswordResponse { Message = "Password successfully reset." };
            return response;
        }

        private static string GenerateRecoveryPasswordLink(string baseUrl)
        {
            var resetLink = $"{baseUrl}recovery-password";
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

        private TokenSettings GetPasswordResetSettings()
        {
            return _configuration.GetSection("TokenSettings").Get<TokenSettings>();
        }
    }
}