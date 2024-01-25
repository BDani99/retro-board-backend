using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RetroBoardBackend.Data;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RetroBoardBackend.Tests.IntegrationTests
{
    public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _context;

        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public UserIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureDeleted();

            var userFaker = UserFakers.CreateUserFaker();
            var users = userFaker.Generate(2);

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            var role = new Role()
            {
                Name = "PM",
                NormalizedName = "PM"
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            foreach (var user in users)
            {
                await _userManager.AddToRoleAsync(user, "PM");
            }

            var token = await _tokenService.GenerateJwtTokenAsync(users.First());
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var projectFaker = ProjectFakers.CreateProjectFaker();
            var project = projectFaker.Generate();

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var retrospectiveFaker = RetrospectiveFakers.CreateRetrospectiveFaker();
            var retrospective = retrospectiveFaker.Generate();

            _context.Retrospectives.Add(retrospective);
            await _context.SaveChangesAsync();

            var entryFaker = EntryFakers.CreateEntryFaker();
            var entry = entryFaker.Generate();

            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("api/Users")]
        public async Task GetAllAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonConvert
                .DeserializeObject<List<UserResponse>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(responseData);
            Assert.Equal(2, responseData.Count);
        }

        [Theory]
        [InlineData("api/Users/1")]
        public async Task GetByIdAsync_CheckResponseStatus_Ok(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Users/3")]
        public async Task GetByIdAsync_CheckResponseStatus_NotFound(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Users/me")]
        public async Task GetMyselfAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Users")]
        public async Task RegisterAsync_CheckResponseStatus_Ok(string url)
        {
            //Arrange
            var dtoFaker = UserFakers.CreateRegisterDtoFaker();
            var registerDto = dtoFaker.Generate();

            var jsonString = System.Text.Json.JsonSerializer.Serialize(registerDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Users")]
        public async Task RegisterAsync_CheckResponseStatus_BadRequest(string url)
        {
            //Arrange
            var registerDto = new RegisterDto
            {
                Email = "asd",
                Password = "asd",
            };

            var jsonString = System.Text.Json.JsonSerializer.Serialize(registerDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Users/my-projects")]
        public async Task GetAllMyProjectAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonConvert
                .DeserializeObject<IEnumerable<MyProjectResponse>>
                (await response.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(responseData);
            Assert.Single(responseData);
        }
    }
}
