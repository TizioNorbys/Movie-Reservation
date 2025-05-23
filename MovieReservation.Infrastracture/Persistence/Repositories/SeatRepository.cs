using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class SeatRepository : RepositoryBase<Seat>
{
	public SeatRepository(MovieDbContext context)
		: base(context)
	{
	}
}