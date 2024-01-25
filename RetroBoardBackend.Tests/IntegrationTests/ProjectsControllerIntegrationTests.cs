using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RetroBoardBackend.Data;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
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
    public class ProjectsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _context;

        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public ProjectsIntegrationTests(CustomWebApplicationFactory<Program> factory)
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
        [InlineData("api/Projects")]
        public async Task GetAllAsync_CheckResponseStatus(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Projects")]
        public async Task GetAllAsync_CheckResponseContentType(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Theory]
        [InlineData("api/Projects")]
        public async Task GetAllAsync_CheckResponseContent(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonConvert
                .DeserializeObject<IEnumerable<ProjectResponse>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(responseData);
            Assert.Single(responseData);
        }

        [Theory]
        [InlineData("api/Projects/1")]
        public async Task GetByIdAsync_CheckResponseStatus_Ok(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Projects/2")]
        public async Task GetByIdAsync_CheckResponseStatus_NotFound(string url)
        {
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Projects/1")]
        public async Task GetByIdAsync_CheckResponseContentType(string url)
        {
            //Act
            var response = await _client.GetAsync(url);
            var responseData = JsonConvert
                .DeserializeObject<ProjectResponse>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}
