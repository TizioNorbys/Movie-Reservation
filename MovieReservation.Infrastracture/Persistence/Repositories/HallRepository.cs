using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class HallRepository : RepositoryBase<Hall>
{
    public HallRepository(MovieDbContext dbContext)
        : base(dbContext)
    {
    }
}