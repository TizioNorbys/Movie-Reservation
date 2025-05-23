using MovieReservation.Domain.Entities;

namespace MovieReservation.Application.Interfaces.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateAsync(User user);
}