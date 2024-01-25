using Bogus;
using RetroBoardBackend.Dtos;

namespace RetroBoardBackend.Tests.Fakers
{
    public class AuthFakers
    {
        public static Faker<LoginDto> CreateLoginDtoFaker()
        {
            return new Faker<LoginDto>()
                .RuleFor(x => x.Email, y => y.Internet.Email())
                .RuleFor(x => x.Password, y => y.Internet.Password());
        }
    }
}
