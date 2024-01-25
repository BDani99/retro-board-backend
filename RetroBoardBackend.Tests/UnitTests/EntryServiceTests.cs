using AutoMapper;
using Moq;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class EntryServiceTests
    {
        [Theory(DisplayName = "GetByIdAsync(int id) - OK")]
        [InlineData(1)]
        public async Task Test_Ok_GetByIdAsync(int id)
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            var mockRepository = new Mock<IEntryRepository>();
            var mockRetrospectiveService = new Mock<IRetrospectiveService>();

            var entryFaker = EntryFakers.CreateEntryFakerWithSpecificId(id);
            var expectedEntry = entryFaker.Generate();

            var entryResponseFaker = EntryFakers.CreateEntryResponseFaker(expectedEntry);
            var expectedResponse = entryResponseFaker.Generate();

            mockRepository
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedEntry);

            mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse.Author);
            mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse.Assignee);

            mockMapper
                .Setup(x => x.Map<EntryResponse>(It.IsAny<Entry>()))
                .Returns(expectedResponse);
            mockMapper
                .Setup(x => x.Map<RetrospectiveResponse>(It.IsAny<Retrospective>()))
                .Returns(expectedResponse.Retrospective);

            mockRetrospectiveService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse.Retrospective);

            var service = new EntryService(mockRepository.Object,
                mockMapper.Object, null, mockRetrospectiveService.Object, mockUserService.Object);

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
            var mockMapper = new Mock<IMapper>();
            var mockRepository = new Mock<IEntryRepository>();

            Entry? expectedEntry = null;
            EntryResponse? expectedResponse = null;

            mockRepository
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedEntry);

            mockMapper
                .Setup(x => x.Map<EntryResponse>(It.IsAny<Entry>()))
                .Returns(expectedResponse);


            var service = new EntryService(mockRepository.Object,
                mockMapper.Object, null, null, null);

            //Act
            var result = await service.GetByIdAsync(id);

            //Assert
            Assert.Equal(expectedResponse, result);
        }

        [Theory(DisplayName = "GetAllCategoriesByIdAsync()")]
        [InlineData(1)]
        public async Task Test_GetAllCategoriesByIdAsync(int id)
        {
            //Arrange
            var mockRepository = new Mock<IEntryRepository>();
            var mockMapper = new Mock<IMapper>();

            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(5);

            var categoryResponses = new List<CategoryResponse>();

            foreach (var category in categories)
            {
                var categoryResponseFaker = CategoryFakers.CreateCategoryResponseFaker(category);
                var categoryResponse = categoryResponseFaker.Generate();

                categoryResponses.Add(categoryResponse);
            }

            mockRepository
                .Setup(x => x.GetAllCategoriesByIdAsync(id))
                .ReturnsAsync(categories);

            mockMapper
                .Setup(x => x.Map<List<CategoryResponse>>(It.IsAny<List<Category>>()))
                .Returns(categoryResponses);

            var service = new EntryService(mockRepository.Object, mockMapper.Object,
                null, null, null);

            //Act
            var result = await service.GetAllCategoriesByIdAsync(id);

            //Assert
            Assert.Equal(categoryResponses, result);
        }

        [Fact(DisplayName = "CreateAsync()")]
        public async Task Test_CreateAsync()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockUserService = new Mock<IUserService>();
            var mockEntryRepository = new Mock<IEntryRepository>();
            var mockRetrospectiveService = new Mock<IRetrospectiveService>();

            var dtoFaker = EntryFakers.CreateDtoFaker();
            var entryDto = dtoFaker.Generate();

            var userFaker = UserFakers.CreateUserFaker();
            var user = userFaker.Generate();

            var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
            var userResponse = userResponseFaker.Generate();

            var entryFaker = EntryFakers.CreateEntryFakerWithDto(entryDto, user.Id);
            var entry = entryFaker.Generate();

            var entryResponseFaker = EntryFakers.CreateEntryResponseFaker(entry);
            var expectedResponse = entryResponseFaker.Generate();

            var authorFaker = UserFakers.CreateUserFakerWithSpecificId(entry.AuthorId);
            var author = authorFaker.Generate();

            var authorResponseFaker = UserFakers.CreateUserResponseFaker(author);
            var authorResponse = authorResponseFaker.Generate();

            var assigneeFaker = UserFakers.CreateUserFakerWithSpecificId(entry.AssigneeId);
            var assignee = assigneeFaker.Generate();

            var assigneeResponseFaker = UserFakers.CreateUserResponseFaker(assignee);
            var assigneeResponse = assigneeResponseFaker.Generate();

            var retrospectiveFaker = RetrospectiveFakers
                .CreateRetrospectiveFakerWithSpecificId(entry.RetrospectiveId);
            var retrospective = retrospectiveFaker.Generate();

            var retrospectiveResponseFaker = RetrospectiveFakers
                .CreateRetrospectiveResponseFaker(retrospective);
            var retrospectiveResponse = retrospectiveResponseFaker.Generate();

            mockMapper
                .Setup(x => x.Map<Entry>(It.IsAny<EntryDto>()))
                .Returns(entry);

            foreach (var category in entry.Categories)
            {
                mockCategoryRepository
                    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(category);
            }

            mockUserService
                .Setup(x => x.GetMyselfAsync())
                .ReturnsAsync(userResponse);

            mockEntryRepository
                .Setup(x => x.CreateAsync(It.IsAny<Entry>()))
                .ReturnsAsync(entry);

            mockMapper
                .Setup(x => x.Map<EntryResponse>(It.IsAny<Entry>()))
                .Returns(expectedResponse);

            mockRetrospectiveService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(retrospectiveResponse);

            mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == user.Id ? userResponse :
                                          id == author.Id ? authorResponse :
                                          id == assignee.Id ? assigneeResponse : null);

            var service = new EntryService(mockEntryRepository.Object,
                mockMapper.Object, mockCategoryRepository.Object, mockRetrospectiveService.Object,
                mockUserService.Object);

            //Act
            var result = await service.CreateAsync(entryDto);

            //Assert
            Assert.Equal(expectedResponse, result);
        }
    }
}