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
    public class EntryReactionServiceTests
    {

        [Fact(DisplayName = "CreateAsync()")]
        public async Task Test_CreateAsync()
        {
            // Arrange
            var userFaker = UserFakers.CreateUserFaker();
            var user = userFaker.Generate();
            var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
            var userResponse = userResponseFaker.Generate();

            var mockUserService = new Mock<IUserService>();
            mockUserService
                .Setup(x => x.GetMyselfAsync())
                .ReturnsAsync(userResponse);

            mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(userResponse);

            var mockEntryService = new Mock<IEntryService>();

            var entryReactionFaker = EntryReactionFakers.CreateEntryReactionFakers(user.Id);

            var expectedEntryReaction = entryReactionFaker.Generate();

            var entryReactionDtoFaker = EntryReactionFakers.CreateEntryReactionDtoFromEntryReactionFakers(expectedEntryReaction);
            var entryReactionDto = entryReactionDtoFaker.Generate();

            var entryReactionResponseFaker = EntryReactionFakers
                .CreateEntryRactionResponseFromEntryReaction(expectedEntryReaction, userResponse);
            var entryReactionResponse = entryReactionResponseFaker.Generate();

            var mockEntryReactionRepository = new Mock<IEntryReactionReposiotry>();
            mockEntryReactionRepository
                .Setup(x => x.CreateAsync(expectedEntryReaction))
                .ReturnsAsync(expectedEntryReaction);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<EntryReaction>(It.IsAny<EntryReactionDto>()))
                .Returns(expectedEntryReaction);

            mockMapper
                .Setup(x => x.Map<EntryReactionResponse>(It.IsAny<EntryReaction>()))
                .Returns(entryReactionResponse);


            var service = new EntryReactionService(mockEntryReactionRepository.Object,
                mockMapper.Object, mockUserService.Object, mockEntryService.Object);

            // Act
            var result = await service.CreateAsync(entryReactionDto);

            // Assert
            Assert.Equal(entryReactionResponse, result);
        }
    }
}