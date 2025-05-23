using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MovieReservation.API;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Infrastracture.Authentication.Handlers;
using MovieReservation.Infrastracture.Persistence;

namespace MovieReservation.Tests.Integration.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Staging);

        builder.ConfigureAppConfiguration(configBuilder =>
            configBuilder.AddUserSecrets<Program>(optional: true));

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContext>();
            var contextOptions = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MovieDbContext>));
            if (contextOptions is not null)
                services.Remove(contextOptions);

            services.AddDbContext<IDbContext, MovieDbContext>((serviceProvider, contextOptions) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                string connectionString = configuration.GetConnectionString("Test")!;

                contextOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
                {
                    options.EnablePrimitiveCollectionsSupport();
                });
            });

            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        }); 
    }
}