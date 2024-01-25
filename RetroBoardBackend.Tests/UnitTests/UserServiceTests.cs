using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class UserServiceTests
    {

        [Fact(DisplayName = "GetAllAsync()")]
        public async Task Test_GetAllAsync()
        {
            // Arrange
            var userFaker = UserFakers.CreateUserFaker();

            var userRoleFaker = UserFakers.CreateUserRoleFaker();

            var expectedUsers = userFaker.Generate(2);

            var mockRepository = new Mock<IUserRepository>();
            mockRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedUsers);

            var expectedResponses = new List<UserResponse>();

            foreach (var user in expectedUsers)
            {
                var expectedResponseFaker = UserFakers.CreateUserResponseFaker(user);
                var expectedResponse = expectedResponseFaker.Generate();

                mockRepository
                    .Setup(x => x.GetUserRoleByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(userRoleFaker.Generate());

                expectedResponses.Add(expectedResponse);
            }

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<List<UserResponse>>(expectedUsers))
                .Returns(expectedResponses);

            var service = new UserService(null, mockMapper.Object, mockRepository.Object, null, null);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(expectedResponses, result.UserResponses);
        }

        [Theory(DisplayName = "GetByIdAsync(int id) - OK")]
        [InlineData(1)]
        public async Task TestOk_GetByIdAsync(int id)
        {
            //Arrange
            var userFaker = UserFakers.CreateUserFakerWithSpecificId(id);

            var userRoleFaker = UserFakers.CreateUserRoleFaker();

            var expectedUser = userFaker.Generate();

            var mockRepository = new Mock<IUserRepository>();

            mockRepository
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedUser);

            mockRepository
                .Setup(x => x.GetUserRoleByIdAsync(id))
                .ReturnsAsync(userRoleFaker.Generate());

            var expectedResponseFaker = UserFakers.CreateUserResponseFaker(expectedUser);
            var expectedResponse = expectedResponseFaker.Generate();

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<UserResponse>(expectedUser))
                .Returns(expectedResponse);

            var service = new UserService(null, mockMapper.Object, mockRepository.Object, null, null);

            //Act
            var result = await service.GetByIdAsync(id);

            //Assert
            Assert.Equal(expectedResponse, result);
        }

        [Theory(DisplayName = "GetByIdAsync(int id) - NOT_FOUND")]
        [InlineData(1)]
        public async Task Test_NotFound_GetByIdAsync(int id)
        {
            //Arrange
            var userRoleFaker = UserFakers.CreateUserRoleFaker();

            User? expectedUser = null;

            var mockRepository = new Mock<IUserRepository>();

            mockRepository.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedUser);

            mockRepository
                .Setup(x => x.GetUserRoleByIdAsync(id))
                .ReturnsAsync(userRoleFaker.Generate());

            UserResponse? expectedResponse = null;

            var mockMapper = new Mock<IMapper>();

            mockMapper
                .Setup(x => x.Map<UserResponse>(expectedUser))
                .Returns(expectedResponse);

            var service = new UserService(null, mockMapper.Object, mockRepository.Object, null, null);

            //Act
            var result = await service.GetByIdAsync(id);

            //Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact(DisplayName = "GetMyselfAsync()")]
        public async Task Test_GetMyselfAsync()
        {
            //Arrange
            var tokenServiceMock = new Mock<ITokenService>();

            tokenServiceMock
                .Setup(service => service.GetMyId())
                .Returns(10);

            var userFaker = UserFakers.CreateUserFakerWithSpecificId(10);

            var userRoleFaker = UserFakers.CreateUserRoleFaker();

            var expectedUser = userFaker.Generate();

            var mockRepository = new Mock<IUserRepository>();

            mockRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(expectedUser);

            mockRepository
                .Setup(x => x.GetUserRoleByIdAsync(10))
                .ReturnsAsync(userRoleFaker.Generate());

            var expectedResponseFaker = UserFakers.CreateUserResponseFaker(expectedUser);
            var expectedResponse = expectedResponseFaker.Generate();

            var mockMapper = new Mock<IMapper>();

            mockMapper
                .Setup(x => x.Map<UserResponse>(expectedUser))
                .Returns(expectedResponse);

            var service = new UserService(null, mockMapper.Object, mockRepository.Object, tokenServiceMock.Object, null);

            //Act
            var result = await service.GetMyselfAsync();

            //Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact(DisplayName = "RegisterAsync() - OK")]
        public async Task Test_Ok_RegisterAsync()
        {
            var dtoFaker = UserFakers.CreateRegisterDtoFaker();
            var registerDto = dtoFaker.Generate();

            var userFaker = UserFakers.CreateUserFakerWithDto(registerDto);
            var expectedUser = userFaker.Generate();

            var mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "PM" });

            var expectedResponseFaker = UserFakers.CreateUserResponseFaker(expectedUser);
            var expectedResponse = expectedResponseFaker.Generate();

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
                .Returns(expectedResponse);

            var service = new UserService(mockUserManager.Object, mockMapper.Object, null, null, null);

            //Act
            var result = await service.RegisterAsync(registerDto);

            //Assert
            Assert.Equal(expectedResponse, result.UserResponse);
        }

        [Fact(DisplayName = "RegisterAsync() - BAD_REQUEST")]
        public async Task Test_BadRequest_RegisterAsync()
        {
            var dtoFaker = UserFakers.CreateRegisterDtoFaker();
            var registerDto = dtoFaker.Generate();

            var userFaker = UserFakers.CreateUserFakerWithDto(registerDto);
            var expectedUser = userFaker.Generate();

            var mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            var errorFaker = UserFakers.CreateIdentityErrorFaker();
            var errors = errorFaker.Generate(2);

            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "PM" });

            var expectedResponseFaker = UserFakers.CreateUserResponseFaker(expectedUser);
            var expectedResponse = expectedResponseFaker.Generate();

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
                .Returns(expectedResponse);

            var service = new UserService(mockUserManager.Object, mockMapper.Object, null, null, null);

            //Act
            var result = await service.RegisterAsync(registerDto);

            //Assert
            Assert.Equal(errors, result.IdentityErrors);
        }

        [Fact(DisplayName = "GetAllMyProjectsAsync")]
        public async Task Test_GetAllMyProjectsAsync()
        {
            //Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockProjectRepository = new Mock<IProjectRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockMapper = new Mock<IMapper>();

            var userFakerWithSpecificId = UserFakers.CreateUserFakerWithSpecificId(10);
            var me = userFakerWithSpecificId.Generate();

            var userResponseFaker = UserFakers.CreateUserResponseFaker(me);
            var myUserResponse = userResponseFaker.Generate();

            var projectFaker = ProjectFakers.CreateProjectFaker();
            var projectsWhereAssignee = projectFaker.Generate(5);

            var projectWhereAuthorFaker = ProjectFakers.CreateProjectWithSpecificAuthorFaker(me.Id, projectsWhereAssignee.Count);
            var projectWhereAuthor = projectWhereAuthorFaker.Generate(2);

            var pmUserRoleFaker = UserFakers.CreatePmUserRoleFaker(me);
            var pmUserRole = pmUserRoleFaker.Generate();

            me.Projects = projectsWhereAssignee;

            var projects = projectsWhereAssignee.Concat(projectWhereAuthor);


            var expectedResult = new List<MyProjectResponse>();

            foreach (var project in projects)
            {
                var myProjectResponseFaker = ProjectFakers.CreateMyProjectResponseFaker(project, me);
                var myProjectResponse = myProjectResponseFaker.Generate();

                mockMapper
                    .Setup(x => x.Map<MyProjectResponse>(project))
                    .Returns(myProjectResponse);

                expectedResult.Add(myProjectResponse);
            }

            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(me);

            mockProjectRepository
                .Setup(x => x.GetProjectsWhereAuthorWithUserIdAsync(myUserResponse.Id))
                .ReturnsAsync(projectWhereAuthor);

            mockMapper
                .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
                .Returns(myUserResponse);

            mockUserRepository
                .Setup(x => x.GetUserRoleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(pmUserRole);

            var service = new UserService(null!, mockMapper.Object, mockUserRepository.Object,
                mockTokenService.Object, mockProjectRepository.Object);

            //Act
            var result = await service.GetAllMyProjectsAsync();

            //Assert
            Assert.Equal(result, expectedResult);
        }
    }
}