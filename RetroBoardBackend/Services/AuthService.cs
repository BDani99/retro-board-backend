using Microsoft.AspNetCore.Identity;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<TokenDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) { return null; }

            var authenticate = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (authenticate)
            {
                return await _tokenService.GenerateJwtTokenAsync(user);
            }

            return null;
        }
    }
}
