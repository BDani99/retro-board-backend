using AutoMapper;
using Moq;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class ProjectServiceTest
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IRetrospectiveRepository> _mockRetrospectiveRepository;
        private readonly Mock<IEntryRepository> _mockEntryRepository;

        private ProjectService service;
        public ProjectServiceTest()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockRetrospectiveRepository = new Mock<IRetrospectiveRepository>();
            _mockEntryRepository = new Mock<IEntryRepository>();

            service = new ProjectService(
                _mockProjectRepository.Object,
                _mockMapper.Object,
                _mockCategoryRepository.Object,
                _mockUserRepository.Object,
                _mockUserService.Object,
                _mockRetrospectiveRepository.Object,
                _mockEntryRepository.Object
            );
        }

        [Fact(DisplayName = "GetAllProjectAsync()")]
        public async Task Test_GetAllProjectAsync()
        {
            //Arrange

            var projectFaker = ProjectFakers.CreateProjectFaker();
            var projects = projectFaker.Generate(5);
            var userRoleFaker = UserFakers.CreateUserRoleFaker();
            var userRole = userRoleFaker.Generate();
            var projectResponses = new List<ProjectResponse>();
            foreach (var project in projects)
            {
                var projectResponseFaker = ProjectFakers.CreateProjectResponseFaker(project);
                var projectResponse = projectResponseFaker.Generate();
                _mockMapper.Setup(x => x.Map<ProjectResponse>(project))
                    .Returns(projectResponse);
                projectResponses.Add(projectResponse);

                _mockUserRepository
                    .Setup(x => x.GetUserRoleByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(userRole);
            }

            var projectResultFaker = ProjectFakers.CreateProjectResult(projectResponses);
            var expectedResult = projectResultFaker.Generate();
            _mockProjectRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(projects);

            //Act
            var result = await service.GetAllProjectAsync();

            //Assert
            Assert.Equal(expectedResult.ProjectResponses, result.ProjectResponses);
        }

        [Fact(DisplayName = "CreateProjectAsync() - OK")]
        public async Task Test_Ok_CreateAsync()
        {
            //Arrange
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(5);

            var userFaker = UserFakers.CreateUserFaker();
            var users = userFaker.Generate(3);
            var authorUser = userFaker.Generate();

            var userRoleFaker = UserFakers.CreateUserRoleFaker();
            var userRole = userRoleFaker.Generate();

            var projectDtoFaker = ProjectFakers.CreateProjectDto(categories, users);
            var projectDto = projectDtoFaker.Generate();

            var projectFaker = ProjectFakers.CreateProjectFromDto(projectDto);
            var project = projectFaker.Generate();

            var postProjectResponseFaker = ProjectFakers.CreatePostProjectResponse(project);
            var postProjectResponse = postProjectResponseFaker.Generate();

            var projectResultFaker = ProjectFakers.CreateProjectResult(postProjectResponse: postProjectResponse);
            var expectedResult = projectResultFaker.Generate();

            _mockMapper
                .Setup(x => x.Map<Project>(projectDto))
                .Returns(project);

            foreach (var category in categories)
            {
                _mockCategoryRepository
                    .Setup(x => x.GetByIdAsync(category.Id))
                    .ReturnsAsync(category);
            }

            foreach (var user in users)
            {
                _mockUserRepository
                    .Setup(x => x.GetByIdAsync(user.Id))
                    .ReturnsAsync(user);

                _mockUserRepository
                    .Setup(x => x.GetUserRoleByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(userRole);
            }

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(authorUser);

            _mockMapper
                .Setup(x => x.Map<PostProjectResponse>(It.IsAny<Project>()))
                .Returns(postProjectResponse);


            //Act
            var result = await service.CreateAsync(projectDto);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "CreateProjectAsync() - CATEGORY_NOT_FOUND_ERROR")]
        public async Task Test_CategoryNotFoundError_CreateAsync()
        {
            //Arrange
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(5);

            var userFaker = UserFakers.CreateUserFaker();
            var users = userFaker.Generate(3);

            var projectDtoFaker = ProjectFakers.CreateProjectDto(categories, users);
            var projectDto = projectDtoFaker.Generate();

            var projectFaker = ProjectFakers.CreateProjectFromDto(projectDto);
            var project = projectFaker.Generate();

            var projectResultFaker = ProjectFakers.CreateProjectResult(errorMessage: "CATEGORY_NOT_FOUND_WITH_THIS_ID");
            var expectedResult = projectResultFaker.Generate();

            _mockMapper
                .Setup(x => x.Map<Project>(projectDto))
                .Returns(project);

            foreach (var category in categories)
            {
                _mockCategoryRepository
                    .Setup(x => x.GetByIdAsync(category.Id))
                    .ReturnsAsync((Category?)null);
            }

            //Act
            var result = await service.CreateAsync(projectDto);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "GetProjectByIdAsync() - OK")]
        public async Task Test_GetProjectByIdAsync_Ok()
        {
            //Arrange
            var projectFaker = ProjectFakers.CreateProjectFaker();
            var project = projectFaker.Generate();

            var projectResponseFaker = ProjectFakers.CreateProjectResponseFaker(project);
            var projectResponse = projectResponseFaker.Generate();

            var userRoleFaker = UserFakers.CreateUserRoleFaker();
            var userRole = userRoleFaker.Generate();

            var projectResultFaker = ProjectFakers.CreateProjectResult(projectResponse: projectResponse);
            var expectedResult = projectResultFaker.Generate();

            _mockProjectRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(project);

            _mockMapper
                .Setup(x => x.Map<ProjectResponse>(It.IsAny<Project>()))
                .Returns(projectResponse);

            _mockUserRepository
                    .Setup(x => x.GetUserRoleByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(userRole);

            //Act
            var result = await service.GetProjectByIdAsync(project.Id);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "GetProjectByIdAsync() - NOT_FOUND")]
        public async Task Test_GetProjectByIdAsync_NotFound()
        {
            //Arrange
            var projectFaker = ProjectFakers.CreateProjectFaker();
            var project = projectFaker.Generate();

            var projectResponseFaker = ProjectFakers.CreateProjectResponseFaker(project);
            var projectResponse = projectResponseFaker.Generate();

            var userRoleFaker = UserFakers.CreateUserRoleFaker();
            var userRole = userRoleFaker.Generate();

            var projectResultFaker = ProjectFakers.CreateProjectResult(errorMessage: "PROJECT_NOT_FOUND_WITH_THIS_ID");
            var expectedResult = projectResultFaker.Generate();

            _mockProjectRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Project?)null);

            //Act
            var result = await service.GetProjectByIdAsync(project.Id);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "UpdateAsync()")]
        public async Task Test_UpdateAsync()
        {
            //Arrange
            var userFaker = UserFakers.CreateUserFaker();
            var me = userFaker.Generate();

            var userResponseFaker = UserFakers.CreateUserResponseFaker(me);
            var meResponse = userResponseFaker.Generate();
            var users = userFaker.Generate(3);

            var projectFaker = ProjectFakers.CreateProjectFaker();
            var originalProject = projectFaker.Generate();

            var updateProjectDtoFaker = ProjectFakers.CreateUpdateProjectDtoFaker();
            var updateProjectDto = updateProjectDtoFaker.Generate();

            var originalProjectResponseFaker = ProjectFakers.CreateProjectResponseFaker(originalProject);
            var originalProjectResponse = originalProjectResponseFaker.Generate();

            var updatedProjectFaker = ProjectFakers.UpdateOriginalResult(updateProjectDto,originalProject);
            var updatedProject = updatedProjectFaker.Generate();

            var updatedProjectResponseFaker = ProjectFakers.CreateProjectResponseFaker(updatedProject);
            var updatedProjectResponse = updatedProjectResponseFaker.Generate();

            var projectResultFaker = ProjectFakers.CreateProjectResult(projectResponse:updatedProjectResponse);
            var expectedResult = projectResultFaker.Generate();

            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(3);

            _mockProjectRepository
                .Setup(x => x.GetByIdAsync(originalProject.Id))
                .ReturnsAsync(originalProject);

            _mockUserService
                .Setup(x => x.GetMyselfAsync())
                .ReturnsAsync(meResponse);

            foreach (var user in users)
            {
                _mockUserRepository
                    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(user);
            }

            foreach (var category in categories)
            {
                _mockCategoryRepository
                    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(category);
            }

            _mockMapper
                .Setup(x => x.Map<ProjectResponse>(It.IsAny<Project>()))
                .Returns(updatedProjectResponse);

            //Act
            var result = await service.UpdateAsync(originalProject.Id,updateProjectDto);

            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
