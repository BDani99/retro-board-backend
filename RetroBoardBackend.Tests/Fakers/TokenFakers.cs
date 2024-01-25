using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;
using RetroBoardBackend.Options;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Tests.Fakers
{
    public class TokenFakers
    {
        private readonly ITokenService _tokenService;

        public TokenFakers(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public static Faker<TokenDto> CreateDtoFaker(User user)
        {
            return new Faker<TokenDto>()
                .CustomInstantiator(f =>
                {
                    var mockManager = new Mock<UserManager<User>>(
                        Mock.Of<IUserStore<User>>(), null, null, null,
                        null, null, null, null, null);
                    var mockOptions = new Mock<IOptions<JwtOptions>>();
                    var mockContextAccessor = new Mock<IHttpContextAccessor>();

                    var jwtOptions = new JwtOptions
                    {
                        Key = "my-super-secret-key",
                        Issuer = "retro-board-backend"
                    };

                    mockManager
                        .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                        .ReturnsAsync(new List<string> { "PM" });

                    mockOptions
                        .Setup(x => x.Value)
                        .Returns(jwtOptions);

                    var tokenService = new TokenService(mockOptions.Object,
                            mockContextAccessor.Object, mockManager.Object);

                    var token = tokenService.GenerateJwtTokenAsync(user).Result;

                    return token;
                });
        }
    }
}
