using AutoMapper;
using Moq;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;
using RetroBoardBackend.Services;
using RetroBoardBackend.Tests.Fakers;

namespace RetroBoardBackend.Tests.UnitTests
{
    public class RetrospectiveServiceTests
    {
        [Theory(DisplayName = "GetByIdAsync(int id) - OK")]
        [InlineData(1)]
        public async Task Test_Ok_GetByIdAsync(int id)
        {
            //Arrange
            var mockRepository = new Mock<IRetrospectiveRepository>();
            var mockMapper = new Mock<IMapper>();

            var retrospectiveFaker = RetrospectiveFakers.CreateRetrospectiveFakerWithSpecificId(id);
            var retrospective = retrospectiveFaker.Generate();

            var retrospectiveResponseFaker = RetrospectiveFakers
                .CreateRetrospectiveResponseFaker(retrospective);
            var expectedResponse = retrospectiveResponseFaker.Generate();

            mockRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(retrospective);

            mockMapper
                .Setup(x => x.Map<RetrospectiveResponse>(It.IsAny<Retrospective>()))
                .Returns(expectedResponse);

            var service = new RetrospectiveService(mockRepository.Object, mockMapper.Object, null);

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
            var mockRepository = new Mock<IRetrospectiveRepository>();
            var mockMapper = new Mock<IMapper>();

            Retrospective? retrospective = null;
            RetrospectiveResponse? expectedResponse = null;

            mockRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(retrospective);

            mockMapper
                .Setup(x => x.Map<RetrospectiveResponse>(It.IsAny<Retrospective>()))
                .Returns(expectedResponse);

            var service = new RetrospectiveService(mockRepository.Object, mockMapper.Object, null);

            //Act
            var result = await service.GetByIdAsync(id);

            //Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact(DisplayName = "CreateAsync()")]
        public async Task Test_CreateAsync()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();
            var mockRepository = new Mock<IRetrospectiveRepository>();

            var dtoFaker = RetrospectiveFakers.CreateDtoFaker();
            var retrospectiveDto = dtoFaker.Generate();

            var retrospectiveFaker = RetrospectiveFakers
                .CreateRetrospectiveFakerWithDto(retrospectiveDto);
            var retrospective = retrospectiveFaker.Generate();

            var retrospectiveResponseFaker = RetrospectiveFakers
                .CreateRetrospectiveResponseFaker(retrospective);
            var expectedResponse = retrospectiveResponseFaker.Generate();

            mockMapper
                .Setup(x => x.Map<Retrospective>(It.IsAny<RetrospectiveDto>()))
                .Returns(retrospective);

            mockRepository
                .Setup(x => x.CreateAsync(It.IsAny<Retrospective>()))
                .ReturnsAsync(retrospective);

            mockMapper
                .Setup(x => x.Map<RetrospectiveResponse>(It.IsAny<Retrospective>()))
                .Returns(expectedResponse);

            var service = new RetrospectiveService(mockRepository.Object, mockMapper.Object, null);

            //Act
            var result = await service.CreateAsync(retrospectiveDto);

            //Assert
            Assert.Equal(expectedResponse, result);
        }
    }
}
