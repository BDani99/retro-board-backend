using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;
using RetroBoardBackend.Options;
using RetroBoardBackend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RetroBoardBackend.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public TokenService(IOptions<JwtOptions> options,
            IHttpContextAccessor contextAccessor,
            UserManager<User> userManager)
        {
            _options = options.Value;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<TokenDto> GenerateJwtTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,roles.FirstOrDefault()!)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var expires = DateTime.Now.AddDays(1);

            var TokenDescriptor = new JwtSecurityToken
            (
                _options.Issuer,
                _options.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(TokenDescriptor),
            };
        }

        public int GetMyId()
        {
            var myToken = new JwtSecurityTokenHandler().ReadJwtToken(
                _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString().Remove(0, 7));
            var myId = Convert.ToInt32(myToken.Claims.First(c => c.Type == "sub").Value);

            return myId;
        }
    }
}
