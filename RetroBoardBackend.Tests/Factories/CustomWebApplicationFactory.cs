using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Services;
using RetroBoardBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore.InMemory;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var dbContextToRemove = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(dbContextToRemove);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.UseInternalServiceProvider(serviceProvider);
            });

            var tokenServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(ITokenService));
            services.Remove(tokenServiceToBeRemoved);
            services.AddScoped<ITokenService, TokenService>();

            var userManagerToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(UserManager<User>));
            services.Remove(userManagerToBeRemoved);
            services.AddScoped<UserManager<User>>();

            var entryServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(IEntryService));
            services.Remove(entryServiceToBeRemoved);
            services.AddScoped<IEntryService, EntryService>();

            var retrospectiveServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(IRetrospectiveService));
            services.Remove(retrospectiveServiceToBeRemoved);
            services.AddScoped<IRetrospectiveService, RetrospectiveService>();

            var userServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(IUserService));
            services.Remove(userServiceToBeRemoved);
            services.AddScoped<IUserService, UserService>();

            var reactionServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(IEntryReactionService));
            services.Remove(reactionServiceToBeRemoved);
            services.AddScoped<IEntryReactionService, EntryReactionService>();

            var projectServiceToBeRemoved = services
                .FirstOrDefault(x => x.ServiceType == typeof(IProjectService));
            services.Remove(projectServiceToBeRemoved);
            services.AddScoped<IProjectService, ProjectService>();
        });

        builder.UseEnvironment("Development");
    }
}