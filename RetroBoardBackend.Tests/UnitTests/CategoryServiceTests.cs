using AutoMapper;
using Moq;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class CategoryServiceTests
    {
        [Fact(DisplayName = "GetAllAsync()")]
        public async Task Test_GetAllAsync()
        {
            //Arrange
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var expectedCategories = categoryFaker.Generate(5);

            var mockRepository = new Mock<ICategoryRepository>();
            mockRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedCategories);

            var expectedResponses = new List<CategoryResponse>();

            foreach (var category in expectedCategories)
            {
                var expectedResponseFaker = CategoryFakers.CreateCategoryResponseFaker(category);
                var expectedResponse = expectedResponseFaker.Generate();

                expectedResponses.Add(expectedResponse);
            }

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<List<CategoryResponse>>(It.IsAny<List<Category>>()))
                .Returns(expectedResponses);

            var service = new CategoryService(mockRepository.Object, null, mockMapper.Object);

            //Act
            var result = await service.GetAllAsync();

            //Assert
            Assert.Equal(expectedResponses, result);
        }

        [Theory(DisplayName = "GetByIdAsync(int id) - OK")]
        [InlineData(1)]
        public async Task Test_Ok_GetByIdAsync(int id)
        {
            //Arrange
            var categoryFaker = CategoryFakers.CreateCategoryFakerWithSpecificId(id);
            var expectedCategory = categoryFaker.Generate();

            var mockRepository = new Mock<ICategoryRepository>();
            mockRepository
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedCategory);

            var expectedResponseFaker = CategoryFakers.CreateCategoryResponseFaker(expectedCategory);
            var expectedResponse = expectedResponseFaker.Generate();

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<CategoryResponse>(It.IsAny<Category>()))
                .Returns(expectedResponse);

            var service = new CategoryService(mockRepository.Object, null, mockMapper.Object);

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
            Category? expectedCategory = null;

            var mockRepository = new Mock<ICategoryRepository>();
            mockRepository
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(expectedCategory);

            CategoryResponse? expectedResponse = null;

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<CategoryResponse>(It.IsAny<Category>()))
                .Returns(expectedResponse);

            var service = new CategoryService(mockRepository.Object, null, mockMapper.Object);

            //Act
            var result = await service.GetByIdAsync(id);

            //Assert
            Assert.Equal(expectedResponse, result);
        }
    }
}
