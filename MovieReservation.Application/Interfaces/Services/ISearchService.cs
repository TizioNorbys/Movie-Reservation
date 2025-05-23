using FluentResults;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.MovieSearch;

namespace MovieReservation.Application.Interfaces.Services;

public interface ISearchService
{
    Task<IEnumerable<MovieDetailsDto>> SearchBy(MovieSearchFiltersDto request, CancellationToken token = default);
}