using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.Extensions;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories.Base;

public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class, IEntityBase
{
    protected readonly MovieDbContext _dbContext;

    protected RepositoryBase(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
    {
        return await _dbContext.Set<T>().ToListAsync(token);
    }

    public virtual async Task<IEnumerable<T>> GetByAsync<K>(Expression<Func<T, K>> propertySelector, K value, CancellationToken token = default)
    {
        var filterExp = propertySelector.GetFilterExpression(value);

        return await _dbContext.Set<T>()
            .Where(filterExp)
            .ToListAsync(token);
    }

    public virtual async Task<IEnumerable<T>> GetByAsync<K>(Expression<Func<T, K>> propertySelector, K value, CancellationToken token = default, params Expression<Func<T, object>>[] navigationSelectors)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        foreach (var navigationSelector in navigationSelectors)
        {
            var navSelector = navigationSelector.CheckAndGetNavigationPropertySelector();
            query = query.Include(navSelector);
        }

        var filterExp = propertySelector.GetFilterExpression(value);
        return await query.Where(filterExp).AsSplitQuery().ToListAsync(token);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _dbContext.Set<T>().FindAsync(new object[] { id }, token);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default, params Expression<Func<T, object>>[] navigationSelectors)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        foreach (var navigationSelector in navigationSelectors)
        {
            var navSelector = navigationSelector.CheckAndGetNavigationPropertySelector();
            query = query.Include(navSelector);
        }

        return await query.AsSplitQuery().SingleOrDefaultAsync(e => e.Id == id, token);
    }

    public virtual async Task<IEnumerable<T>> GetByMultipleValuesAsync<K>(Expression<Func<T, K>> propertySelector, IEnumerable<K> values, CancellationToken token = default)
    {
        var filterExpression = propertySelector.GetFilterExpression(values);

        return await _dbContext.Set<T>()
            .Where(filterExpression) 
            .ToListAsync(token);
    }

    public virtual async Task<IEnumerable<K>> GetRelatedAsync<K, U>(Guid id, Expression<Func<T, ICollection<K>>> navigationSelector, Expression<Func<K, U>>? sortKeySelector = null, CancellationToken token = default)
        where K : IEntityBase
    {
        var selector = navigationSelector.CheckAndGetCollectionNavigationPropertySelector();

        if (sortKeySelector is null)
            return await _dbContext.Set<T>().Where(e => e.Id == id).SelectMany(selector).ToListAsync(token);

        var keySelector = sortKeySelector.CheckAndGetSortKeySelector();
        return await _dbContext.Set<T>().Where(e => e.Id == id).SelectMany(selector).OrderBy(keySelector).ToListAsync(token);
    }

    public virtual async Task<bool> ExistByAsync<K>(Expression<Func<T, K>> propertySelector, K value, CancellationToken token = default)
    {
        var filterExp = propertySelector.GetFilterExpression(value);

        return await _dbContext.Set<T>().AnyAsync(filterExp, token);
    }

    public virtual void Update(T entity)
    {   
        _dbContext.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public virtual async Task<int> CountAsync(CancellationToken token = default) => await _dbContext.Set<T>().CountAsync(token);

    public void Attach(T entity) => _dbContext.Set<T>().Attach(entity);

    public void SetValues(T entity, object dto)
    {
        var entityEntry = _dbContext.Entry(entity);

        if (entityEntry.State == EntityState.Detached)
            Attach(entity);

        entityEntry.CurrentValues.SetValues(dto);
    }

    public void SetPropertyToModified<K>(T entity, Expression<Func<T, K>> propertySelector)
    {
        var selector = propertySelector.CheckAndGetPropertySelector();
        var entityEntry = _dbContext.Entry(entity);

        if (entityEntry.State == EntityState.Detached)
            Attach(entity);

        entityEntry.Property(selector).IsModified = true;
    }
}