using System.Linq.Expressions;
using MovieReservation.Application.Extensions;

namespace MovieReservation.Application.Utility;

public static class FilterBuilder<T>
{
    public static Expression<Func<T, bool>> BuildSingleFilterBy<K>(Expression<Func<T, K>> propertySelector, K value)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        var comparisonExpression = propertySelector.Body.GetComparisonExpression(Expression.Constant(value, typeof(K)), typeof(T));

        return Expression.Lambda<Func<T, bool>>(comparisonExpression, propertySelector.Parameters);
    }

    public static Expression<Func<T, bool>> BuildRangeFilterBy<K>(Expression<Func<T, K>> propertySelector, K min, K max)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        var minExp = Expression.Constant(min, typeof(K));
        var maxExp = Expression.Constant(max, typeof(K));

        var greaterExp = Expression.GreaterThanOrEqual(propertySelector.Body, minExp);
        var lessExp = Expression.LessThanOrEqual(propertySelector.Body, maxExp);
        var andExp = Expression.AndAlso(greaterExp, lessExp);

        return Expression.Lambda<Func<T, bool>>(andExp, propertySelector.Parameters);
    }
}