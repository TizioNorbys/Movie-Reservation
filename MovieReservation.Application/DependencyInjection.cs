using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.Validators.Actor;

namespace MovieReservation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IActorService, ActorService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IShowtimeService, ShowtimeService>();
        services.AddScoped<IReservationService, ReservationService>();

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection), ServiceLifetime.Scoped,
            filter => filter.ValidatorType != typeof(CreateUpdateActorDtoValidator));
        services.AddTransient<IValidator<CreateUpdateActorDto>, CreateUpdateActorDtoValidator>();
        
        return services;
    }
}