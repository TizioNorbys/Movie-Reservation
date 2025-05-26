using System.Linq.Expressions;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.MovieSearch;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Utility;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;

namespace MovieReservation.Application.Services;

public class SearchService : ISearchService
{
    private readonly IMovieRepository _movieRepository;

    public SearchService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<IEnumerable<MovieDetailsDto>> SearchBy(MovieSearchFiltersDto request, CancellationToken token = default)
    {
        var movies = await _movieRepository.SearchByAsync(filters: GetFilterExpressions(request), token);
        return movies.ToDtos();
    }

    private static List<Expression<Func<Movie, bool>>> GetFilterExpressions(MovieSearchFiltersDto filters)
    {
        List<Expression<Func<Movie, bool>>> filterExpressions = new();

        if (filters.Title is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildSingleFilterBy(m => m.Title, filters.Title));

        if (filters.MinRating is not null && filters.MaxRating is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildRangeFilterBy(m => m.Rating, filters.MinRating, filters.MaxRating));

        if (filters.Genres is not null)
        {
            Expression<Func<Movie, bool>> genresFilterExp = m => filters.Genres.All(inputGenre => m.Genres.Select(g => g.Name).Contains(inputGenre));
            filterExpressions.Add(genresFilterExp);
        }

        if (filters.MinRuntime is not null && filters.MaxRuntime is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildRangeFilterBy(m => m.RunningTime, filters.MinRuntime, filters.MaxRuntime));

        if (filters.MinYear is not null && filters.MaxYear is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildRangeFilterBy(m => m.ReleaseYear, filters.MinYear, filters.MaxYear));

        if (filters.Language is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildSingleFilterBy(m => m.OriginalLanguage, filters.Language));

        if (filters.Country is not null)
            filterExpressions.Add(FilterBuilder<Movie>.BuildSingleFilterBy(m => m.Country, filters.Country));

        return filterExpressions;
    }
}