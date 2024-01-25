using Bogus;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class EntryReactionFakers
    {
        public static Faker<EntryReaction> CreateEntryReactionFakers(int id)
        {
            return new Faker<EntryReaction>()
                .RuleFor(x => x.UserId, id)
                .RuleFor(x => x.EntryId, y => y.IndexFaker + 1)
                .RuleFor(x => x.ReactionTypes, y => y.PickRandom<ReactionTypes>());
        }

        public static Faker<EntryReactionDto> CreateEntryReactionDtoFromEntryReactionFakers(EntryReaction entryReaction)
        {
            return new Faker<EntryReactionDto>()
                .RuleFor(x => x.EntryId, entryReaction.EntryId)
                .RuleFor(x => x.ReactionType, entryReaction.ReactionTypes.ToString());
        }

        public static Faker<EntryReactionDto> CreateEntryReactionDto()
        {
            return new Faker<EntryReactionDto>()
                .RuleFor(x => x.EntryId, y => y.IndexFaker + 1)
                .RuleFor(x => x.ReactionType, y => y.PickRandom<ReactionTypes>().ToString());
        }

        public static Faker<EntryReaction> CreateEntryReactionWithSpecificIds(int userId, int entryId)
        {
            return new Faker<EntryReaction>()
                .RuleFor(x => x.UserId, userId)
                .RuleFor(x => x.EntryId, entryId)
                .RuleFor(x => x.ReactionTypes, y => y.PickRandom<ReactionTypes>());
        }

        public static Faker<EntryReactionResponse>
            CreateEntryRactionResponseFromEntryReaction(EntryReaction entryReaction, UserResponse userResponse)
        {
            //var userFaker = UserFakers.CreateUserFakerWithSpecificId(entryReaction.UserId);
            //var user = userFaker.Generate();

            //var userResponseFaker = UserFakers.CreateUserResponseFaker(user);
            //var userResponse = userResponseFaker.Generate();

            var entryFaker = EntryFakers.CreateEntryFaker();
            var entry = entryFaker.Generate();

            var entryResponseFaker = EntryFakers.CreateEntryResponseFaker(entry);
            var entryResponse = entryResponseFaker.Generate();

            return new Faker<EntryReactionResponse>()
                .RuleFor(x => x.Id, entryReaction.Id)
                .RuleFor(x => x.ReactionType, entryReaction.ReactionTypes.ToString())
                .RuleFor(x => x.User, userResponse)
                .RuleFor(x => x.Entry, entryResponse);
        }


    }
}
