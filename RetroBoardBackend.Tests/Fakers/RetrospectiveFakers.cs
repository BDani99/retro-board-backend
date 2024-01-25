using Bogus;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Enums;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class RetrospectiveFakers
    {
        public static Faker<Retrospective> CreateRetrospectiveFaker()
        {
            return new Faker<Retrospective>()
                .RuleFor(x => x.Id, f => f.IndexFaker + 1)
                .RuleFor(x => x.ProjectId, f => f.IndexFaker + 1)
                .RuleFor(x => x.Name, y => y.Random.String(7))
                .RuleFor(x => x.CreatedAt, DateTime.Now)
                .RuleFor(x => x.StatsNeeded, y => y.Random.Bool())
                .RuleFor(x => x.IsActive, y => y.Random.Bool());
        }

        public static Faker<Retrospective> CreateRetrospectiveFakerWithSpecificId(int id)
        {
            return new Faker<Retrospective>()
                .RuleFor(x => x.Id, id)
                .RuleFor(x => x.ProjectId, f => f.IndexFaker + 1)
                .RuleFor(x => x.Name, y => y.Random.String(7))
                .RuleFor(x => x.CreatedAt, DateTime.Now)
                .RuleFor(x => x.StatsNeeded, y => y.Random.Bool())
                .RuleFor(x => x.IsActive, y => y.Random.Bool());
        }

        public static Faker<RetrospectiveDto> CreateDtoFaker()
        {
            return new Faker<RetrospectiveDto>()
                .RuleFor(x => x.Name, y => y.Random.String())
                .RuleFor(x => x.StatsNeeded, y => y.Random.Bool())
                .RuleFor(x => x.ProjectId, y => y.IndexFaker + 1);
        }

        public static Faker<Retrospective> CreateRetrospectiveFakerWithDto(RetrospectiveDto retrospectiveDto)
        {
            return new Faker<Retrospective>()
                .RuleFor(x => x.Id, f => f.IndexFaker + 1)
                .RuleFor(x => x.ProjectId, retrospectiveDto.ProjectId)
                .RuleFor(x => x.Name, retrospectiveDto.Name)
                .RuleFor(x => x.CreatedAt, DateTime.Now)
                .RuleFor(x => x.StatsNeeded, retrospectiveDto.StatsNeeded)
                .RuleFor(x => x.IsActive, false);
        }

        public static Faker<RetrospectiveResponse> CreateRetrospectiveResponseFaker(Retrospective retrospective)
        {
            var needsFixColumnAmount = retrospective.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.NeedsFix)).Count();

            var wentWellColumnAmount = retrospective.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.WentWell)).Count();

            var todoAmount = retrospective.Entries
                .Where(x => x.ColumnType.Equals(ColumnTypes.Todo)).Count();

            return new Faker<RetrospectiveResponse>()
                .RuleFor(x => x.Id, retrospective.Id)
                .RuleFor(x => x.Name, retrospective.Name)
                .RuleFor(x => x.CreatedAt, retrospective.CreatedAt)
                .RuleFor(x => x.StatsNeeded, retrospective.StatsNeeded)
                .RuleFor(x => x.IsActive, retrospective.IsActive)
                .RuleFor(x => x.EntryAmount, new EntryAmount
                {
                    NeedsFixColumn = needsFixColumnAmount,
                    WentWellColumn = wentWellColumnAmount,
                    TodoColumn = todoAmount,
                    Total = needsFixColumnAmount +
                            wentWellColumnAmount +
                            todoAmount
                });
        }
    }
}