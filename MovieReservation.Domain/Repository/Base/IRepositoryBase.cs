using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Repository.Base
{
	public interface IRepositoryBase<T>
		where T : class, IEntityBase
	{
		void Add(T entity);

		Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

		Task<IEnumerable<T>> GetByAsync<K>(Expression<Func<T, K>> propertySelector, K Value, CancellationToken token = default);

		Task<IEnumerable<T>> GetByAsync<K>(Expression<Func<T, K>> propertySelector, K value, CancellationToken token = default, params Expression<Func<T, object>>[] navigationSelectors);

		Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);

		Task<T?> GetByIdAsync(Guid id, CancellationToken token = default, params Expression<Func<T, object>>[] navigationSelectors);

		Task<IEnumerable<T>> GetByMultipleValuesAsync<K>(Expression<Func<T, K>> propertySelector, IEnumerable<K> values, CancellationToken token = default);

		Task<IEnumerable<K>> GetRelatedAsync<K, U>(Guid id, Expression<Func<T, ICollection<K>>> navigationSelector, Expression<Func<K, U>>? sortKeySelector = null, CancellationToken token = default)
			where K : IEntityBase;

		Task<bool> ExistByAsync<K>(Expression<Func<T, K>> propertySelector, K value, CancellationToken token = default);

		void Update(T entity);

		void Delete(T entity);

		Task<int> CountAsync(CancellationToken token = default);

		void Attach(T entity);

		void SetValues(T entity, object dto);

		void SetPropertyToModified<K>(T Entity, Expression<Func<T, K>> propertySelector);
	}
}