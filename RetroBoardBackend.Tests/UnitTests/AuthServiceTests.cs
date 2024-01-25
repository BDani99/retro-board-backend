using Microsoft.AspNetCore.Identity;
using Moq;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class AuthServiceTests
    {
        [Fact(DisplayName = "LoginAsync() - OK")]
        public async Task Test_Ok_LoginAsync()
        {
            //Arrange
            var userFaker = UserFakers.CreateUserFaker();
            var expectedUser = userFaker.Generate();

            var mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            var dtoFaker = AuthFakers.CreateLoginDtoFaker();
            var loginDto = dtoFaker.Generate();

            var mockTokenService = new Mock<ITokenService>();

            var expectedDto = await mockTokenService.Object.GenerateJwtTokenAsync(expectedUser);

            mockTokenService
                .Setup(x => x.GenerateJwtTokenAsync(expectedUser))
                .ReturnsAsync(expectedDto);

            mockUserManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedUser);

            mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "PM" });

            mockUserManager
                .Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var service = new AuthService(mockUserManager.Object, mockTokenService.Object);

            //Act
            var result = await service.LoginAsync(loginDto);

            //Assert
            Assert.Equal(expectedDto, result);
        }

        [Fact(DisplayName = "LoginAsync() - NOT_FOUND")]
        public async Task Test_NotFound_LoginAsync()
        {
            //Arrange
            User? expectedUser = null;

            var mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            var dtoFaker = AuthFakers.CreateLoginDtoFaker();
            var loginDto = dtoFaker.Generate();

            var mockTokenService = new Mock<ITokenService>();

            var expectedDto = await mockTokenService.Object.GenerateJwtTokenAsync(expectedUser);

            mockTokenService
                .Setup(x => x.GenerateJwtTokenAsync(expectedUser))
                .ReturnsAsync(expectedDto);

            mockUserManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedUser);

            mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "PM" });

            mockUserManager
                .Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var service = new AuthService(mockUserManager.Object, mockTokenService.Object);

            //Act
            var result = await service.LoginAsync(loginDto);

            //Assert
            Assert.Equal(expectedDto, result);
        }
    }
}
