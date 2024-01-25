using Bogus;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class ReactionFakers
    {
        public static Faker<EntryReaction> CreateReactionFaker()
        {
            //User
            var userFaker = UserFakers.CreateUserFaker();
            var user = userFaker.Generate();

            //Entry
            var entryFaker = EntryFakers.CreateEntryFaker();
            var entry = entryFaker.Generate();

            return new Faker<EntryReaction>()
                .RuleFor(x => x.Id, f => f.IndexFaker + 1)
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.EntryId, entry.Id)
                .RuleFor(x => x.ReactionTypes, y => y.PickRandom<ReactionTypes>())
                .RuleFor(x => x.User, user);
        }
    }
}