using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class ReviewRepository : RepositoryBase<Review>
{
    public ReviewRepository(MovieDbContext dbContext)
        : base(dbContext)
    {
    }
}