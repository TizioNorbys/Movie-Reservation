using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Exceptions;

namespace MovieReservation.Application.Extensions;

public static class LambdaExpressionsExtensions
{
    public static string GetPropertyName(this LambdaExpression lambda)
    {
        ArgumentNullException.ThrowIfNull(lambda);

        MemberExpression me = lambda.Body as MemberExpression ?? throw new InvalidSelectorExpressionException(lambda);

        return me.Member.Name;
    }

    public static BinaryExpression GetComparisonExpression(this Expression left, Expression right, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        MemberExpression me = left as MemberExpression ?? throw new InvalidSelectorExpressionException(left);
        if (!me.IsValidMemberExpression(entityType))
            throw new InvalidPropertySelectorException(left, entityType);

        return Expression.Equal(left, right);
    }

    public static MethodCallExpression GetContainsCallExpression<K>(this Expression arg1, IEnumerable<K> arg0, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(arg1);

        MemberExpression me = arg1 as MemberExpression ?? throw new InvalidSelectorExpressionException(arg1);
        if (!me.IsValidMemberExpression(entityType))
            throw new InvalidPropertySelectorException(arg1, entityType);

        var containsMethodInfo = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(mi => mi.Name == nameof(Enumerable.Contains) && mi.GetParameters().Length == 2).First()
            .MakeGenericMethod(typeof(K));

        return Expression.Call(null, containsMethodInfo, Expression.Constant(arg0), arg1);
    }

    public static Expression<Func<T, bool>> GetFilterExpression<T, K>(this Expression<Func<T, K>> propertySelector, K value)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        var filter = propertySelector.Body.GetComparisonExpression(Expression.Constant(value, typeof(K)), typeof(T));
        return Expression.Lambda<Func<T, bool>>(filter, propertySelector.Parameters);
    }

    public static Expression<Func<T, bool>> GetFilterExpression<T, K>(this Expression<Func<T, K>> propertySelector, IEnumerable<K> values)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        var filter = propertySelector.Body.GetContainsCallExpression(values, typeof(T));
        return Expression.Lambda<Func<T, bool>>(filter, propertySelector.Parameters);
    }

    public static Expression<Func<T, K>> CheckAndGetNavigationPropertySelector<T, K>(this Expression<Func<T, K>> navigationSelector)
    {
        ArgumentNullException.ThrowIfNull(navigationSelector);

        MemberExpression me = navigationSelector.Body as MemberExpression ?? throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));
        if (me.Member.DeclaringType != typeof(T))
            throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));

        if (!IsNavigationProperty(me.Type))
            throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));

        return navigationSelector;
    }

    public static Expression<Func<T, IEnumerable<K>>> CheckAndGetCollectionNavigationPropertySelector<T, K>(this Expression<Func<T, ICollection<K>>> navigationSelector)
        where K : IEntityBase
    {
        ArgumentNullException.ThrowIfNull(navigationSelector);

        MemberExpression me = navigationSelector.Body as MemberExpression ?? throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));
        if (me.Member.DeclaringType != typeof(T))
            throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));

        var argumentType = me.Type.GetFirstGenericArgumentType();
        if (!argumentType.IsAssignableTo(typeof(IEntityBase)) && argumentType != typeof(SeatReservation))
            throw new InvalidNavigationPropertySelectorException(navigationSelector, typeof(T));

        return Expression.Lambda<Func<T, IEnumerable<K>>>(me, navigationSelector.Parameters);
    }

    public static Expression<Func<T, K>> CheckAndGetPropertySelector<T, K>(this Expression<Func<T, K>> propertySelector)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        MemberExpression me = propertySelector.Body as MemberExpression ?? throw new InvalidSelectorExpressionException(propertySelector);
        if (!me.IsValidMemberExpression(typeof(T)))
            throw new InvalidPropertySelectorException(propertySelector, typeof(T));

        return propertySelector;
    }

    public static Expression<Func<T, K>> CheckAndGetSortKeySelector<T, K>(this Expression<Func<T, K>> propertySelector)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);

        MemberExpression me = propertySelector.Body as MemberExpression ?? throw new InvalidSelectorExpressionException(propertySelector);
        if (!(me.IsValidMemberExpression(typeof(T)) || typeof(IEnumerable<IEntityBase>).IsAssignableFrom(me.Member.DeclaringType)))
            throw new InvalidPropertySelectorException(propertySelector, typeof(T));

        return propertySelector;
    }

    private static bool IsValidMemberExpression(this MemberExpression me, Type entityType)
    {
        var type = me.Member.DeclaringType;

        return type == entityType
            || type == typeof(EntityBase)
            || type == typeof(IdentityUser<Guid>)
            || type == typeof(IdentityRole<Guid>);
    }

    private static Type GetFirstGenericArgumentType(this Type type)
    {
        if (!type.IsGenericType)
            throw new ArgumentException($"{type.Name} type is not a generic type");

        return type.GetGenericArguments()[0];
    }

    private static bool IsNavigationProperty(Type propertyType)
    {
        var baseType = typeof(IEntityBase);

        if (propertyType.IsAssignableTo(baseType))
            return true;

        if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType.IsGenericType)
        {
            var argumentType = propertyType.GetFirstGenericArgumentType();
            return argumentType.IsAssignableTo(baseType) || argumentType == typeof(SeatReservation);
        }
            
        return false;
    }
}