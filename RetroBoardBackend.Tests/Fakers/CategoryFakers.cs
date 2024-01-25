using Bogus;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Tests.Fakers
{
    public class CategoryFakers
    {
        public static Faker<Category> CreateCategoryFaker()
        {
            return new Faker<Category>()
                .RuleFor(x => x.Id, f => f.IndexFaker + 1)
                .RuleFor(x => x.Name, y => y.Random.String(7))
                .RuleFor(x => x.Color, y => y.Internet.Color());
        }

        public static Faker<Category> CreateCategoryFakerWithSpecificId(int id)
        {
            return new Faker<Category>()
                .RuleFor(x => x.Id, id)
                .RuleFor(x => x.Name, y => y.Random.String(7))
                .RuleFor(x => x.Color, y => y.Internet.Color());
        }

        public static Faker<CategoryResponse> CreateCategoryResponseFaker(Category category)
        {
            return new Faker<CategoryResponse>()
                .RuleFor(x => x.Id, category.Id)
                .RuleFor(x => x.Name, category.Name)
                .RuleFor(x => x.Color, category.Color);
        }
    }
}