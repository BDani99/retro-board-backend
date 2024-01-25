using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IProjectRepository _projectRepository;

        public UserService(UserManager<User> userManager, IMapper mapper,
            IUserRepository userRepository, ITokenService tokenService, IProjectRepository projectRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _projectRepository = projectRepository;
        }

        public async Task<UserResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User()
            {
                Email = registerDto.Email,
                Image = registerDto.Image,
                UserName = registerDto.UserName
            };

            if (registerDto.Image != null)
            {
                user.Image = registerDto.Image;
            }

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new UserResult { IdentityErrors = result.Errors };
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "PM");

                var getUserResponse = _mapper.Map<UserResponse>(user);
                getUserResponse.Role = (await _userManager.GetRolesAsync(user)).First();
                return new UserResult
                {
                    UserResponse = getUserResponse
                };
            }
        }

        public async Task UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            _mapper.Map(updateUserDto, user);

            await _userRepository.SaveChangesAsync();
        }

        public async Task<UserResult> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userResponses = _mapper.Map<List<UserResponse>>(users);

            foreach (var userResponse in userResponses)
            {
                var role = await _userRepository.GetUserRoleByIdAsync(userResponse.Id);
                if (role == null)
                {
                    return new UserResult { ErrorMessage = ErrorMessages.THERE_IS_NO_ROLE_FOR_THIS_USER };
                }

                userResponse.Role = role.Role.Name;
            }

            return new UserResult { UserResponses = userResponses };
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) { return null; }

            var userResponse = _mapper.Map<UserResponse>(user);

            var role = await _userRepository.GetUserRoleByIdAsync(userResponse.Id);
            if (role == null)
            {
                return null;
            }

            userResponse.Role = role.Role.Name;
            return userResponse;
        }

        public async Task<UserResponse?> GetMyselfAsync()
        {
            var myId = _tokenService.GetMyId();


            var userResponse = await GetByIdAsync(myId);

            var role = await _userRepository.GetUserRoleByIdAsync(myId);
            if (role == null || userResponse == null)
            {
                return null;
            }

            userResponse.Role = role.Role.Name;

            return userResponse;
        }

        public async Task<IEnumerable<MyProjectResponse>?> GetAllMyProjectsAsync()
        {
            var myUserResponse = await GetMyselfAsync();
            if (myUserResponse is null)
            {
                return null;
            }

            var me = await _userRepository.GetByIdAsync(myUserResponse.Id);
            if (me is null)
            {
                return null;
            }

            var projectsWhereAuthor = await _projectRepository.GetProjectsWhereAuthorWithUserIdAsync(myUserResponse.Id);

            var projectsWhereAssignee = me.Projects.Where(x => x.IsDeleted == false).ToList();

            var allMyProjects = projectsWhereAssignee;

            allMyProjects.AddRange(projectsWhereAuthor);

            var myProjectsResponse = new List<MyProjectResponse>();
            foreach (var project in allMyProjects)
            {
                var myProjectResponse = _mapper.Map<MyProjectResponse>(project);
                var role = await _userRepository.GetUserRoleByIdAsync(myProjectResponse.AuthorUser.Id);
                if (role == null)
                {
                    return null;
                }

                myProjectResponse.AuthorUser.Role = role.Role.Name;
                myProjectsResponse.Add(myProjectResponse);
            }

            return myProjectsResponse.GroupBy(x => x.Id).Select(x => x.First()).ToList();
        }
    }
}
