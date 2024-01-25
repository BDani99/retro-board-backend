using Bogus;
using Microsoft.AspNetCore.Identity;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class UserFakers
    {
        public static Faker<User> CreateUserFaker()
        {
            return new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Image, f => f.Internet.Avatar())
                .RuleFor(u => u.SecurityStamp, Guid.NewGuid().ToString())
                .RuleFor(u => u.Email, f => f.Internet.Email());
        }

        public static Faker<User> CreateUserFakerWithSpecificId(int id)
        {
            return new Faker<User>()
                .RuleFor(u => u.Id, id)
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Image, f => f.Internet.Avatar())
                .RuleFor(u => u.SecurityStamp, Guid.NewGuid().ToString())
                .RuleFor(u => u.Email, f => f.Internet.Email());
        }

        public static Faker<UserRole> CreateUserRoleFaker()
        {
            return new Faker<UserRole>()
                .RuleFor(ur => ur.Role, f => new Role());
        }
        public static Faker<UserRole> CreatePmUserRoleFaker(User user)
        {
            return new Faker<UserRole>()
                .RuleFor(ur => ur.Role, f => new Role() { Id = 1, Name = "PM" })
                .RuleFor(x => x.RoleId, 1)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.UserId, user.Id);
        }


        public static Faker<UserResponse> CreateUserResponseFaker(User user)
        {
            return new Faker<UserResponse>()
                .RuleFor(u => u.Id, user.Id)
                .RuleFor(u => u.Username, user.UserName)
                .RuleFor(u => u.Image, user.Image)
                .RuleFor(u => u.Email, user.Email);
        }

        public static Faker<RegisterDto> CreateRegisterDtoFaker()
        {
            return new Faker<RegisterDto>()
                .RuleFor(x => x.Email, y => y.Internet.Email())
                .RuleFor(x => x.UserName, y => y.Internet.UserName())
                .RuleFor(x => x.Password, y => y.Internet.Password() + y.Random.Number() + "/")
                .RuleFor(u => u.ConfirmPassword, (f, u) => u.Password)
                .RuleFor(x => x.Image, y => y.Internet.Avatar());
        }

        public static Faker<User> CreateUserFakerWithDto(RegisterDto registerDto)
        {
            return new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.UserName, registerDto.UserName)
                .RuleFor(u => u.Image, registerDto.Image)
                .RuleFor(u => u.SecurityStamp, Guid.NewGuid().ToString())
                .RuleFor(u => u.Email, registerDto.Email);
        }

        public static Faker<IdentityError> CreateIdentityErrorFaker()
        {
            return new Faker<IdentityError>()
                .RuleFor(x => x.Code, y => "")
                .RuleFor(x => x.Description, y => "");
        }

        public static Faker<UserResponse> CreateUserResponseFakerWithProject(User user)
        {
            return new Faker<UserResponse>()
                .RuleFor(u => u.Id, user.Id)
                .RuleFor(u => u.Username, user.UserName)
                .RuleFor(u => u.Image, user.Image)
                .RuleFor(u => u.Email, user.Email);
        }
    }
}
