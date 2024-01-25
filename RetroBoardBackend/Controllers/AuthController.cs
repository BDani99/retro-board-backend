using Microsoft.AspNetCore.Mvc;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RetroBoardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenDto>> Post([FromBody] LoginDto loginDto)
        {
            var tokenDto = await _authService.LoginAsync(loginDto);
            if (tokenDto is not null)
            {
                return Ok(tokenDto);
            }

            return BadRequest(new { error = ErrorMessages.INVALID_EMAIL_OR_PASSWORD });
        }
    }
}
