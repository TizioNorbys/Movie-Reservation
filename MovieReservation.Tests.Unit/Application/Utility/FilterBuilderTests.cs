using System.Linq.Expressions;
using MovieReservation.Application.Utility;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Exceptions;

namespace MovieReservation.Tests.Unit.Application.Utility;

public class FilterBuilderTests
{
    private static readonly object testObject = new();
    private static readonly Review testReview = new();

    [Fact]
    public void BuildSingleFilter_ReturnsValidFilterExpression()
    {
        // Arrange
        Expression<Func<Movie, bool>> expected = m => m.Title == "title";
        // Act
        var actual = FilterBuilder<Movie>.BuildSingleFilterBy(m => m.Title, "title");
        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void BuildRangeFilter_ReturnsValidFilterExpression()
    {
        // Arrange
        Expression<Func<Movie, bool>> expected = m => m.Rating >= 2 && m.Rating <= 8;
        // Act
        var actual = FilterBuilder<Movie>.BuildRangeFilterBy(m => m.Rating, 2, 8);
        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void BuildSingleFilter_ThrowsException_When_ExpressionIsNotPropertySelector()
    {
        Assert.Throws<InvalidSelectorExpressionException>(() =>
            FilterBuilder<Movie>.BuildSingleFilterBy(m => new(), testObject));
    }

    [Fact]
    public void BuildSingleFilter_ThrowException_When_SelectorDoesNotSelectPropertyOfType()
    {
        Assert.Throws<InvalidPropertySelectorException>(() =>
            FilterBuilder<Movie>.BuildSingleFilterBy(m => testReview.Content, testObject));
    }

    [Fact]
    public void BuildSingleFilter_ThrowsException_When_SelectorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FilterBuilder<Movie>.BuildSingleFilterBy(null!, testObject));
    }

    [Fact]
    public void BuildRangeFilter_ThrowsException_When_SelectorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FilterBuilder<Movie>.BuildRangeFilterBy(null!, testObject, testObject));
    }
}
