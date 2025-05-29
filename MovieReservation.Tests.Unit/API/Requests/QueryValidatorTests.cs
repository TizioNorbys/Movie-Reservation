using MovieReservation.API.Requests.QueryModels;
using MovieReservation.Domain.Constants;

namespace MovieReservation.Tests.Unit.API.Requests;

public class QueryValidatorTests
{
    [Theory]
    [InlineData("5555", "&rating=1,10")]
    [InlineData("rating", "&rating=1,10")]
    [InlineData(",", "&rating=1,10")]   
    [InlineData(".,", "&rating=1,10")]
    [InlineData(",.", "&rating=1,10")]
    [InlineData(".,.", "&rating=1,10")]
    [InlineData("5,", "&rating=5,10")]
    [InlineData(",5", "&rating=1,5")]
    [InlineData("0.5555,10", "&rating=1,10")]
    [InlineData("0,5555", "&rating=1,10")]
    [InlineData("5.4321,5.6789", "&rating=5.4,5.6")]
    [InlineData("1.000000000,9.9999999999", "&rating=1,9.9")]
    [InlineData("2.77,3.18", "&rating=2.7,3.1")]
    [InlineData("8,3", "&rating=3,8")]
    [InlineData("5,5", "&rating=5,5")]
    [InlineData("7,9", "&rating=7,9")] 
    public void ValidateMovieRating_Returns_CorrectRatingQueryParam(string rating, string expected)
    {
        string actual = QueryValidator.ValidateMovieRating(rating);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Italian", "&language=italian")]
    [InlineData("ITALIAN", "&language=italian")]
    [InlineData("italian", "&language=italian")]
    [InlineData("----italian", "")]
    [InlineData("A", "")]
    [InlineData("language", "")]
    public void ValidateWordProperty_Returns_CorrectLanguageQueryParam(string input, string expected)
    {
        string actual = QueryValidator.ValidateWordProperty<MovieSearchQuery, Languages>(q => q.Language!, input);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(",,,,", "")]
    [InlineData("1234", "")]
    [InlineData("genre", "")]
    [InlineData("first,second,third", "")]
    [InlineData("action,drama,genre,", "&genres=action,drama")]
    [InlineData("first,Action,second,Drama", "&genres=action,drama")]
    [InlineData("drama,", "&genres=drama")]
    [InlineData("Drama", "&genres=drama")]
    [InlineData("DRAMA", "&genres=drama")]
    public void ValidateListProperty_Returns_CorrectGenresQueryParam(string input, string expected)
    {
        string actual = QueryValidator.ValidateListProperty<MovieSearchQuery, MovieGenres>(q => q.Genres!, input);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("runtime", "")]
    [InlineData("100", "")]
    [InlineData(",", "&runtime=1,100000")]
    [InlineData("5,", "&runtime=5,100000")]
    [InlineData(",5555", "&runtime=1,5555")]
    [InlineData("0,555", "&runtime=1,555")]
    [InlineData("55,555555", "&runtime=55,100000")]
    [InlineData("555,55", "&runtime=55,555")]
    public void ValidateRangeProperty_Returns_CorrectRuntimeQueryParam(string input, string expected)
    {
        string actual = QueryValidator.ValidateRangeProperty<MovieSearchQuery>(q => q.Runtime!, input, min: 1, max: 100_000);

        Assert.Equal(expected, actual);
    }
}