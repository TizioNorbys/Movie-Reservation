using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Requests.QueryModels;
using MovieReservation.Application.Interfaces.Services;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/search")]
public class MovieSearchController : ControllerBase
{
	private readonly IMovieService _movieService;
	private readonly ISearchService _searchService;

	public MovieSearchController(ISearchService searchService, IMovieService movieService)
	{
		_searchService = searchService;
		_movieService = movieService;
	}

	[HttpGet("{title}")] 
	public async Task<IActionResult> SearchByTitle(string title, CancellationToken token = default)
	{
		if (string.IsNullOrWhiteSpace(title))
			return NotFound();

		var movies = await _movieService.GetBestMatchesByTitle(title, 6, token);
		return movies.Any() ? Ok(movies) : NotFound(new { Message = $"No movies found for {title} title" });
	}

    [HttpGet]
	public async Task<IActionResult> SearchBy([FromQuery] MovieSearchQuery searchFilters, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(Request.QueryString.Value))
            return NoContent();

        string queryString = searchFilters.Validate();
		if (queryString != Request.QueryString.Value?.TrimStart('?'))
		{
            var url = Url.Action(ControllerContext.ActionDescriptor.ActionName, ControllerContext.ActionDescriptor.ControllerName, null, Request.Scheme, Request.Host.Value);
            return Redirect($"{url}?{queryString}");
		}

		var result = await _searchService.SearchBy(searchFilters.ToDto(), token);
		return result.Any() ? Ok(result) : NotFound(new { Message = "No result" });
    }
}