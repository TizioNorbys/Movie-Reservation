namespace MovieReservation.Application.Interfaces.Data;

public interface IDbContext
{
    Task<int> SaveChangesAsync(CancellationToken token = default);
}