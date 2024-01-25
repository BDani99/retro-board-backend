using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroBoardBackend.Data;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RetroBoardBackend.Tests.IntegrationTests
{
    public class EntriesIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEntryService _entryService;
        private readonly IRetrospectiveService _retrospectiveService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public EntriesIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
            _entryService = _scope.ServiceProvider.GetRequiredService<IEntryService>();
            _retrospectiveService = _scope.ServiceProvider.GetRequiredService<IRetrospectiveService>();
            _userService = _scope.ServiceProvider.GetRequiredService<IUserService>();
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureDeleted();

            var userFaker = UserFakers.CreateUserFaker();
            var user = userFaker.Generate();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var role = new Role()
            {
                Name = "PM",
                NormalizedName = "PM"
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, "PM");

            var token = await _tokenService.GenerateJwtTokenAsync(user);
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var retrospectiveDtoFaker = RetrospectiveFakers.CreateDtoFaker();
            var retrospectiveDto = retrospectiveDtoFaker.Generate();

            var retrospectiveFaker = RetrospectiveFakers
                .CreateRetrospectiveFakerWithDto(retrospectiveDto);
            var retrospective = retrospectiveFaker.Generate();

            _context.Retrospectives.Add(retrospective);
            await _context.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("api/Entries")]
        public async Task CreateAsync_CheckResponseStatus(string url)
        {
            //Arrange
            var dtoFaker = EntryFakers.CreateDtoFaker();
            var entryDto = dtoFaker.Generate();

            var jsonString = JsonSerializer.Serialize(entryDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = dataToSend
            };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Entries")]
        public async Task CreateAsync_CheckResponseContentType(string url)
        {
            //Arrange
            var dtoFaker = EntryFakers.CreateDtoFaker();
            var entryDto = dtoFaker.Generate();

            var jsonString = JsonSerializer.Serialize(entryDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = dataToSend
            };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Theory]
        [InlineData("api/Entries/1/categories")]
        public async Task GetAllCategoriesByIdAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Entries/1/categories")]
        public async Task GetAllCategoriesByIdAsync_CheckResponseContentType(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Theory]
        [InlineData("api/Entries/1/categories")]
        public async Task GetAllCategoriesByIdAsync_CheckResponseContent(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonSerializer
                .Deserialize<List<CategoryResponse>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(responseData);
            Assert.Empty(responseData);
        }
    }
}
