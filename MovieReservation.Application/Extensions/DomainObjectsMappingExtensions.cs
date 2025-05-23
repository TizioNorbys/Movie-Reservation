using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Application.Extensions;

public static class DomainObjectsMappingExtensions
{
    public static MovieDetailsDto ToDto(this Movie movie) => new(movie.Title, movie.Description, movie.Plot, movie.ReleaseYear, movie.OriginalLanguage, movie.Country, movie.RunningTime, movie.Rating, movie.Genres.Select(g => g.Name));

    public static ReviewDetailsDto ToDto(this Review review)
    {
        int likes = review.Reactions.Where(r => r.Type).Count();
        int dislikes = review.Reactions.Count - likes;

        return new(review.Rating, review.Title, review.Content, review.Timestamp, likes, dislikes);
    }

    public static ActorDetailsDto ToDto(this Actor actor) => new(actor.FullName, actor.BirthDate);

    public static ShowtimeDetailsDto ToDto(this Showtime showtime, IShowtimeSeatsQueryResult showtimeSeatsStatus) => new(showtime.Timestamp, showtime.Hall.Number, showtimeSeatsStatus.AvailableSeats, showtimeSeatsStatus.SeatsStatus);

    public static IEnumerable<MovieDetailsDto> ToDtos(this IEnumerable<Movie> movies)
    {
        return movies.Select(m =>
            new MovieDetailsDto(m.Title, m.Description, m.Plot, m.ReleaseYear, m.OriginalLanguage, m.Country, m.RunningTime, m.Rating, m.Genres.Select(g => g.Name)));
    }

    public static IEnumerable<ReviewDetailsDto> ToDtos(this IEnumerable<Review> reviews)
    {
        foreach (var r in reviews)
        {
            int likes = r.Reactions.Where(r => r.Type).Count();
            int dislikes = r.Reactions.Count - likes;

            yield return new ReviewDetailsDto(r.Rating, r.Title, r.Content, r.Timestamp, likes, dislikes);
        }
    }

    public static IEnumerable<ActorDetailsDto> ToDtos(this IEnumerable<Actor> actors)
    {
        return actors.Select(a => new ActorDetailsDto(a.FullName, a.BirthDate));
    }

    public static IEnumerable<ShowtimeInfoDto> ToDtos(this IEnumerable<Showtime> showtimes)
    {
        return showtimes.Select(s => new ShowtimeInfoDto { Timestamp = s.Timestamp });
    }

    public static IEnumerable<ShowtimeReservationDto> ToDtos(this IEnumerable<Reservation> reservations)
    {
        return reservations.Select(r => new ShowtimeReservationDto
        {
            Code = r.Code,
            UserName = r.User.UserName!,
            SeatsReserved = r.Seats.Select(s => s.Number)
        });
    }
}