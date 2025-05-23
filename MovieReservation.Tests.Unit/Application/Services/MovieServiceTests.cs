using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MovieReservation.API.Extensions;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Services;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using NSubstitute;

namespace MovieReservation.Tests.Unit.Application.Services;

public class MovieServiceTests
{
    private readonly MovieService _sut;
    private readonly IMovieRepository _movieRepositoryStub;
    private readonly IValidator<CreateUpdateMovieDto> _movieValidatorStub;
    private readonly ILogger<MovieService> _loggerStub;

    private static readonly Guid testId = Guid.NewGuid();

    public MovieServiceTests()
    {
        _movieRepositoryStub = Substitute.For<IMovieRepository>();
        _movieValidatorStub = Substitute.For<IValidator<CreateUpdateMovieDto>>();
        _loggerStub = Substitute.For<ILogger<MovieService>>();

        _sut = new MovieService(_movieRepositoryStub, null!, null!, null!, null!, _movieValidatorStub, null!, _loggerStub);
    }

    [Fact]
    public async Task Create_ReturnsValidationError_WhenModelValidationFails()
    {
        // Arrange
        var testRequest = new CreateUpdateMovieDto(string.Empty, string.Empty, string.Empty, default, string.Empty, string.Empty, default, default, new List<string>());
        ValidationResult testValResult = new();
        testValResult.Errors.Add(new(string.Empty, string.Empty));

        _movieValidatorStub.Validate(testRequest)
            .Returns(testValResult);

        // Act
        var actual = await _sut.Create(testRequest);

        // Assert
        Assert.True(actual.IsFailed);
        Assert.IsType<ValidationError>(actual.GetFirstError());
    }

    [Theory]
    [InlineData(ReviewSortValues.Rating, SortingOrders.Desc, "r => Convert(r.Rating, Object)", SortingOrders.Desc)]
    [InlineData(ReviewSortValues.Reactions, SortingOrders.Asc, "r => Convert(r.Reactions.Count, Object)", SortingOrders.Asc)]
    [InlineData(ReviewSortValues.Date, SortingOrders.Desc, "r => Convert(r.Timestamp, Object)", SortingOrders.Desc)]
    [InlineData("test", SortingOrders.Asc, "r => Convert(r.Rating, Object)", SortingOrders.Asc)]
    [InlineData(ReviewSortValues.Rating, "test", "r => Convert(r.Rating, Object)", SortingOrders.Desc)]
    [InlineData("test", "test", "r => Convert(r.Rating, Object)", SortingOrders.Desc)]
    public async Task GetReviews_CallsGetReviewsAsync_WithCorrectArguments(string sort, string sortOrder, string expectedSortExp, string expectedSortOrder)
    {
        // Arrange
        _movieRepositoryStub.ExistByAsync(Arg.Any<Expression<Func<Movie, Guid>>>(), testId, default)
            .Returns(true);

        // Act
        await _sut.GetReviews(testId, sort, sortOrder, null, default);

        // Assert
        await _movieRepositoryStub.Received(1)
            .GetReviewsAsync(
                testId,
                Arg.Is<Expression<Func<Review, object>>>(exp => exp.ToString() == expectedSortExp),
                expectedSortOrder,
                null,
                default);
    }
}