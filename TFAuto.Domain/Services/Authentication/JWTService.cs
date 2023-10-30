using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.Authentication.Constants;
using TFAuto.Domain.Services.Authentication.Models;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.Authentication;

public class JWTService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly IRepository<Role> _repositoryRole;
    private readonly JWTSettings _jwtSettings;

    public JWTService(IRepository<User> repositoryUser, IRepository<Role> repositoryRole, IOptions<JWTSettings> jwtSettings)
    {
        _repositoryUser = repositoryUser;
        _repositoryRole = repositoryRole;
        _jwtSettings = jwtSettings.Value;
    }

    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public async Task<List<Claim>> GetClaims(bool isAccessToken, string userId, string email)
    {
        var user = await _repositoryUser.GetAsync(c => c.Id == userId).FirstOrDefaultAsync();
        var role = await _repositoryRole.GetAsync(c => c.Id == user.RoleId).FirstOrDefaultAsync();
        var claims = new List<Claim>
        {
            new Claim(CustomClaimsType.USER_ID, userId),
            new Claim(CustomClaimsType.USER_NAME, user.UserName),
            new Claim(CustomClaimsType.EMAIL, email),
            new Claim(CustomClaimsType.IS_ACCESS, isAccessToken.ToString()),
            new Claim(CustomClaimsType.ROLE_ID, user.RoleId)
        };

        foreach (var permissionid in role.PermissionIds)
        {
            claims.Add(new Claim(CustomClaimsType.PERMISSION_ID, permissionid));
        }

        return claims;
    }

    public JwtSecurityToken CreateToken(List<Claim> claims, int lifetime)
    {
        return new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(lifetime),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_jwtSettings.IssuerSigningKey), SecurityAlgorithms.HmacSha256));
    }

    public async Task<Token> GenerateTokenMode(string userId, string email)
    {
        var claims = await GetClaims(true, userId, email);
        var accessToken = CreateToken(claims, _jwtSettings.AccessTokenLifetimeInSeconds);

        claims = await GetClaims(false, userId, email);
        var refreshToken = CreateToken(claims, _jwtSettings.RefreshTokenLifetimeInSeconds);

        var now = DateTime.UtcNow;
        var tokenModel = new Token()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken),
            AccessTokenExpireDate = now.AddSeconds(_jwtSettings.AccessTokenLifetimeInSeconds),
            RefreshTokenExpireDate = now.AddSeconds(_jwtSettings.RefreshTokenLifetimeInSeconds)
        };
        return tokenModel;
    }
}
