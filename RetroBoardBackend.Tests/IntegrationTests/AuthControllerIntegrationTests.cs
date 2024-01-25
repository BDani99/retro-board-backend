using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroBoardBackend.Data;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services.Interfaces;
using RetroBoardBackend.Tests.Fakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetroBoardBackend.Tests.IntegrationTests
{
    public class AuthIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>,
        IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        private string? _email;
        private string? _password;

        public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureDeleted();
            var dtoFaker = UserFakers.CreateRegisterDtoFaker();
            var registerDto = dtoFaker.Generate();

            var userFaker = UserFakers.CreateUserFakerWithDto(registerDto);
            var user = userFaker.Generate();

            _email = registerDto.Email;
            _password = registerDto.Password;

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            var role = new Role()
            {
                Name = "PM",
                NormalizedName = "PM"
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, "PM");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("api/Auth")]
        public async Task Post_CheckResponseStatus_Ok(string url)
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Email = _email,
                Password = _password
            };

            var jsonString = JsonSerializer.Serialize(loginDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Auth")]
        public async Task Post_CheckResponseStatus_BadRequest(string url)
        {
            var loginDto = new LoginDto
            {
                Email = "asd",
                Password = "asd"
            };

            var jsonString = JsonSerializer.Serialize(loginDto);
            var dataToSend = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = dataToSend };

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("api/Auth")]
        public async Task Post_CheckResponseContentType(string url)
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Email = _email,
                Password = _password
            };

            var jsonString = JsonSerializer.Serialize(loginDto);
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
