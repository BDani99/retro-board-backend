using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<RegisterDto> _validator;

        public UsersController(IUserService userService,
            IValidator<RegisterDto> validator)
        {
            _userService = userService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAllAsync()
        {
            var userResult = await _userService.GetAllAsync();
            if (userResult.ErrorMessage != null)
            {
                return BadRequest(new { Error = userResult.ErrorMessage });
            }
            return Ok(userResult.UserResponses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetByIdAsync(int id)
        {
            var userResponse = await _userService.GetByIdAsync(id);
            if (userResponse == null)
            {
                return NotFound(new { Error = ErrorMessages.USER_NOT_FOUND_WITH_THIS_ID });
            }
            return Ok(userResponse);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetMyselfAsync()
        {
            var me = await _userService.GetMyselfAsync();
            return Ok(me);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            var validatorResult = await _validator.ValidateAsync(registerDto);
            if (!validatorResult.IsValid) return BadRequest(validatorResult.Errors);

            var userResult = await _userService.RegisterAsync(registerDto);
            return userResult.UserResponse != null ? Ok(userResult.UserResponse) : BadRequest(userResult.IdentityErrors);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { error = ErrorMessages.USER_NOT_FOUND_WITH_THIS_ID });
            }

            await _userService.UpdateAsync(id, updateUserDto);

            return NoContent();
        }

        [HttpGet("my-projects")]
        public async Task<ActionResult<IEnumerable<MyProjectResponse>>> GetAllMyProjectAsync()
        {
            var myProjects = await _userService.GetAllMyProjectsAsync();
            if (myProjects is null)
            {
                return BadRequest(ErrorMessages.THE_USER_OR_THE_PROJECT_IS_NULL);
            }
            return Ok(myProjects);
        }
    }
}
