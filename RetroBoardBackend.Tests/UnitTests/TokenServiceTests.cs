using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using RetroBoardBackend.Models;
using RetroBoardBackend.Options;
using RetroBoardBackend.Services;
using RetroBoardBackend.Tests.Fakers;
using System.IdentityModel.Tokens.Jwt;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class TokenServiceTests
    {
        [Fact(DisplayName = "GenerateJwtTokenAsync()")]
        public async Task Test_GenerateJwtTokenAsync()
        {
            var mockManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null,
                null, null, null, null, null);
            var mockOptions = new Mock<IOptions<JwtOptions>>();
            var mockContextAccessor = new Mock<IHttpContextAccessor>();

            var userFaker = UserFakers.CreateUserFaker();
            var user = userFaker.Generate();

            var roles = new List<string>() { "PM" };

            var dtoFaker = TokenFakers.CreateDtoFaker(user);
            var tokenDto = dtoFaker.Generate();

            var jwtOptions = new JwtOptions
            {
                Key = "my-super-secret-key",
                Issuer = "retro-board-backend"
            };

            mockManager
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(roles);

            mockOptions
                .Setup(x => x.Value)
                .Returns(jwtOptions);

            var expectedToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenDto.AccessToken);
            var expectedSub = expectedToken.Claims.First(x => x.Type == "sub").Value;
            var expectedEmail = expectedToken.Claims.First(x => x.Type == "email").Value;

            var service = new TokenService(mockOptions.Object,
                mockContextAccessor.Object, mockManager.Object);

            //Act
            var result = await service.GenerateJwtTokenAsync(user);
            var resultToken = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
            var resultSub = resultToken.Claims.First(x => x.Type == "sub").Value;
            var resultEmail = resultToken.Claims.First(x => x.Type == "email").Value;

            //Assert
            Assert.Equal(expectedSub, resultSub);
            Assert.Equal(expectedEmail, resultEmail);
        }
    }
}
