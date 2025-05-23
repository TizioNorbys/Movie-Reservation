using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class ReservationRepository : RepositoryBase<Reservation>
{
	public ReservationRepository(MovieDbContext context)
		: base(context)
	{
	}
}