using FrontStage.Dto;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FrontStage.Service
{
    public class IdentityService
    {
        private readonly ILogger<IdentityService> _log;
        private readonly JwtConfig _jwtConfig;

        public IdentityService(IOptions<JwtConfig> jwtConfig, ILogger<IdentityService> log)
        {
            _jwtConfig = jwtConfig.Value;
            _log = log;
        }

        public IdentityResultDto GenerateToken(string role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role,role),
                };

                var securityToken = new JwtSecurityToken(
                    issuer: _jwtConfig.Issuer,
                    audience: _jwtConfig.Audience,
                    claims: claims,
                    notBefore: _jwtConfig.NotBefore,
                    expires: _jwtConfig.Expiration,
                    signingCredentials: _jwtConfig.SigningCredentials
                );

                var access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);

                return new IdentityResultDto()
                {
                    AccessToken = $"Bearer {access_token}",
                };
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }
    }
}
