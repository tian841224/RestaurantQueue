using FrontStage.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FrontStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IdentityController : ControllerBase
    {
        private readonly JwtConfig _jwtConfig;

        public IdentityController(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        [HttpGet]
        public IdentityResultDto Login(string role)
        {
            return GenerateToken(role);
        }

        private IdentityResultDto GenerateToken(string role)
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
    }
}
