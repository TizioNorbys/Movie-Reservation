using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class GenreRepository : RepositoryBase<Genre>
{
	public GenreRepository(MovieDbContext dbContext)
		: base(dbContext)
	{
	}
}