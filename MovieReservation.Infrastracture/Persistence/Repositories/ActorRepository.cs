using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class ActorRepository : RepositoryBase<Actor>
{
    public ActorRepository(MovieDbContext dbContext)
        : base(dbContext)
    {
    }
}