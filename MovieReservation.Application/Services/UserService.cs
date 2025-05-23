using Microsoft.Extensions.Logging;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Repository;

namespace MovieReservation.Application.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly ILogger<UserService> _logger;

	public UserService(IUserRepository userRepository, ILogger<UserService> logger)
	{
		_userRepository = userRepository;
		_logger = logger;
	}

    public async Task<IEnumerable<IReservationDetailsQueryResult>> GetReservations(Guid id, CancellationToken token = default)
    {
		_logger.LogInformation("Fetching reservation of user with {Id} id", id);
		return await _userRepository.GetReservationsAsync(id, token);
    }
}