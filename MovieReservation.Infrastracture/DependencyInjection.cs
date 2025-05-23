using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieReservation.Application.Interfaces.Authentication;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;
using MovieReservation.Infrastracture.Authentication;
using MovieReservation.Infrastracture.Persistence;
using MovieReservation.Infrastracture.Persistence.Repositories;

namespace MovieReservation.Infrastracture;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastracture(this IServiceCollection services)
	{
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
        services.AddScoped<IRepositoryBase<Review>, ReviewRepository>();
        services.AddScoped<IRepositoryBase<Seat>, SeatRepository>();
        services.AddScoped<IRepositoryBase<Reservation>, ReservationRepository>();
        services.AddScoped<IRepositoryBase<Actor>, ActorRepository>();
        services.AddScoped<IRepositoryBase<Genre>, GenreRepository>();
        services.AddScoped<IRepositoryBase<Hall>, HallRepository>();

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddDbContext<IDbContext, MovieDbContext>((serviceProvider, contextOptions) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string connectionString = configuration.GetConnectionString("Default")!;

            contextOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.EnablePrimitiveCollectionsSupport();
            });

            contextOptions.EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });

        services.AddDefaultIdentity<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 10;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<MovieDbContext>();

		return services;
	}
}