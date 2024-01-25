using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroBoardBackend.Data;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
using SQLitePCL;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RetroBoardBackend.Tests.IntegrationTests
{
    public class CategoriesIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public CategoriesIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
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

            var categoryFaker = CategoryFakers.CreateCategoryFaker();
            var categories = categoryFaker.Generate(10);

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("api/Categories")]
        public async Task GetAllAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Categories")]
        public async Task GetAllAsync_CheckResponseContent(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonSerializer
                .Deserialize<List<CategoryResponse>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(responseData);
            Assert.Equal(10, responseData.Count);
        }

        [Theory]
        [InlineData("api/Categories")]
        public async Task GetAllAsync_CheckResponseHeaderContentType(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}
