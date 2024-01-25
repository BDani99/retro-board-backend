using Bogus;
using Moq;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class EntryFakers
    {
        public static Faker<EntryDto> CreateDtoFaker()
        {
            var categoryIds = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                categoryIds.Add(new int());
            }

            return new Faker<EntryDto>()
                .RuleFor(x => x.AssigneeId, y => y.IndexFaker + 1)
                .RuleFor(x => x.RetrospectiveId, y => y.IndexFaker + 1)
                .RuleFor(x => x.EntryContent, y => y.Random.String())
                .RuleFor(x => x.ColumnType, y => y.PickRandom<ColumnTypes>().ToString())
                .RuleFor(x => x.CategoryIds, categoryIds);
        }

        public static Faker<Entry> CreateEntryFaker()
        {
            return new Faker<Entry>()
                .RuleFor(x => x.Id, y => y.IndexFaker + 1)
                .RuleFor(x => x.AuthorId, y => y.IndexFaker + 1)
                .RuleFor(x => x.AssigneeId, y => y.IndexFaker + 1)
                .RuleFor(x => x.RetrospectiveId, y => y.IndexFaker + 1)
                .RuleFor(x => x.EntryContent, y => y.Commerce.Department())
                .RuleFor(x => x.ColumnType, y => y.PickRandom<ColumnTypes>());
        }

        public static Faker<Entry> CreateEntryFakerWithDto(EntryDto entryDto, int authorId)
        {
            ColumnTypes? columnType = null;
            switch (entryDto.ColumnType)
            {
                case "Todo":
                    columnType = ColumnTypes.Todo;
                    break;
                case "WentWell":
                    columnType = ColumnTypes.WentWell;
                    break;
                case "NeedsFix":
                    columnType = ColumnTypes.NeedsFix;
                    break;
            }

            return new Faker<Entry>()
                .RuleFor(x => x.Id, y => y.IndexFaker + 1)
                .RuleFor(x => x.AuthorId, authorId)
                .RuleFor(x => x.AssigneeId, entryDto.AssigneeId)
                .RuleFor(x => x.RetrospectiveId, entryDto.RetrospectiveId)
                .RuleFor(x => x.EntryContent, entryDto.EntryContent)
                .RuleFor(x => x.ColumnType, columnType);
        }

        public static Faker<Entry> CreateEntryFakerWithSpecificId(int id)
        {
            return new Faker<Entry>()
                .RuleFor(x => x.Id, id)
                .RuleFor(x => x.AuthorId, y => y.IndexFaker + 1)
                .RuleFor(x => x.AssigneeId, y => y.IndexFaker + 1)
                .RuleFor(x => x.RetrospectiveId, y => y.IndexFaker + 1)
                .RuleFor(x => x.EntryContent, y => y.Random.String())
                .RuleFor(x => x.ColumnType, y => y.PickRandom<ColumnTypes>());
        }

        public static Faker<Entry> CreateEntryFakerWithSpecificIds(int id, int authorId,
            int assigneeId, int retrospectiveId)
        {
            return new Faker<Entry>()
                .RuleFor(x => x.Id, id)
                .RuleFor(x => x.AuthorId, authorId)
                .RuleFor(x => x.AssigneeId, assigneeId)
                .RuleFor(x => x.RetrospectiveId, retrospectiveId)
                .RuleFor(x => x.EntryContent, y => y.Random.String())
                .RuleFor(x => x.ColumnType, y => y.PickRandom<ColumnTypes>());
        }

        public static Faker<EntryResponse> CreateEntryResponseFaker(Entry entry)
        {
            //Category
            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(5);

            var categoryResponses = new List<CategoryResponse>();

            foreach (var category in categories)
            {
                var categoryResponseFaker = CategoryFakers.CreateCategoryResponseFaker(category);
                var categoryResponse = categoryResponseFaker.Generate();

                categoryResponses.Add(categoryResponse);
            }

            //Retrospective
            var retrospectiveFaker = RetrospectiveFakers
                .CreateRetrospectiveFakerWithSpecificId(entry.RetrospectiveId);
            var retrospective = retrospectiveFaker.Generate();

            var retrospectiveResponseFaker = RetrospectiveFakers
                   .CreateRetrospectiveResponseFaker(retrospective);
            var retrospectiveResponse = retrospectiveResponseFaker.Generate();

            //Author
            var authorFaker = UserFakers.CreateUserFakerWithSpecificId(entry.AuthorId);
            var author = authorFaker.Generate();

            var authorResponseFaker = UserFakers.CreateUserResponseFaker(author);
            var authorResponse = authorResponseFaker.Generate();

            //Assignee
            var assigneeFaker = UserFakers.CreateUserFakerWithSpecificId(entry.AssigneeId);
            var assignee = assigneeFaker.Generate();

            var assigneeResponseFaker = UserFakers.CreateUserResponseFaker(assignee);
            var assigneeResponse = assigneeResponseFaker.Generate();

            //Reactions
            var likeAmount = entry.EntryReactions
                .Where(x => x.ReactionTypes.Equals(ReactionTypes.Like)).Count();

            var dislikeAmount = entry.EntryReactions
                .Where(x => x.ReactionTypes.Equals(ReactionTypes.Dislike)).Count(); ;

            return new Faker<EntryResponse>()
                .RuleFor(x => x.Id, entry.Id)
                .RuleFor(x => x.EntryContent, entry.EntryContent)
                .RuleFor(x => x.Categories, categoryResponses)
                .RuleFor(x => x.Retrospective, retrospectiveResponse)
                .RuleFor(x => x.Author, authorResponse)
                .RuleFor(x => x.Assignee, assigneeResponse)
                .RuleFor(x => x.ColumnType, entry.ColumnType.ToString())
                .RuleFor(x => x.ReactionAmount, new ReactionAmount
                {
                    LikeAmount = likeAmount,
                    DislikeAmount = dislikeAmount
                })
                .RuleFor(x => x.CurrentUserReaction, It.IsAny<string>());
        }
    }
}


