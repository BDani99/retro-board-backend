using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetroBoardBackend.Tests.IntegrationTests
{
    public class ReactionIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _context;

        private readonly IEntryReactionService _reactionService;
        private readonly IEntryService _entryService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public ReactionIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _reactionService = _scope.ServiceProvider.GetRequiredService<IEntryReactionService>();
            _entryService = _scope.ServiceProvider.GetRequiredService<IEntryService>();
            _userService = _scope.ServiceProvider.GetRequiredService<IUserService>();
            _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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
            var categories = categoryFaker.Generate(5);

            _context.Categories.AddRange(categories);
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
        [InlineData("api/Reactions")]
        public async Task CreateAsync_CheckResponseStatus(string url)
        {
            //Arrange
            var dtoFaker = EntryReactionFakers.CreateEntryReactionDto();
            var reactionDto = dtoFaker.Generate();

            var jsonString = JsonSerializer.Serialize(reactionDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Reactions")]
        public async Task CreateAsync_CheckResponseContentType(string url)
        {
            //Arrange
            var dtoFaker = EntryReactionFakers.CreateEntryReactionDto();
            var reactionDto = dtoFaker.Generate();

            var jsonString = JsonSerializer.Serialize(reactionDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}
