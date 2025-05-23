using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Application.Extensions;

public static class DtosMappingExtensions
{
    public static Movie ToMovie(this CreateUpdateMovieDto dto) => new(dto.Title, dto.Description, dto.Plot, dto.ReleaseYear, dto.OriginalLanguage.ToLower().FirstLetterToUpper(), dto.Country.ToLower().FirstLetterToUpper(), dto.RunningTime, dto.Rating);

    public static Actor ToActor(this CreateUpdateActorDto dto) => new(dto.FullName, dto.BirthDate);

    public static Actor ToActor(this CreateUpdateActorDto dto, Guid id) => new(id, dto.FullName, dto.BirthDate);

    public static Review ToReview(this CreateUpdateReviewDto dto, Guid movieId, Guid userId) => new(dto.Rating, dto.Title, dto.Content, DateTime.UtcNow, movieId, userId);

    public static Showtime ToShowtime(this CreateShowtimeDto dto) => new(dto.Timestamp, dto.HallId, dto.MovieId);
}