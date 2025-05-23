using MovieReservation.API.Requests.Actor;
using MovieReservation.API.Requests.Auth;
using MovieReservation.API.Requests.Movie;
using MovieReservation.API.Requests.Reservation;
using MovieReservation.API.Requests.Review;
using MovieReservation.API.Requests.Showtime;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Auth;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.Reservation;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.API.Extensions;

public static class RequestModelsMappingExtensions
{
	public static CreateUpdateMovieDto ToDto(this CreateUpdateMovieRequest request) => new(request.Title, request.Description, request.Plot, request.ReleaseYear, request.OriginalLanguage.ToLower().FirstLetterToUpper(), request.Country.ToLower().FirstLetterToUpper(), request.RunningTime, request.Rating, request.Genres.Select(g => g.ToLower().FirstLetterToUpper()));

    public static CreateUpdateReviewDto ToDto(this CreateUpdateReviewRequest request) => new(request.Rating, request.Title, request.Content);

    public static CreateUpdateActorDto ToDto(this CreateUpdateActorRequest request) => new(request.FullName, request.BirthDate);

    public static CreateShowtimeDto ToDto(this CreateShowtimeRequest request) => new(request.Timestamp, request.HallId, request.MovieId);

    public static UpdateShowtimeDto ToDto(this UpdateShowtimeRequest request) => new(request.Timestamp, request.MovieId);
 
    public static CreateReservationDto ToDto(this CreateReservationRequest request) => new(request.ShowtimeId, request.SeatsIds);

    public static RegistrationDto ToDto(this RegistrationRequest request) => new(request.Email, request.FirstName, request.LastName, request.Password, request.ConfirmPassword);

    public static LoginDto ToDto(this LoginRequest request) => new(request.Email, request.Password);
}