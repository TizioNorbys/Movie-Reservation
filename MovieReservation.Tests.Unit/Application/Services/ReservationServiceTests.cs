using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MovieReservation.API.Extensions;
using MovieReservation.Application.DTOs.Reservation;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Authorization;
using MovieReservation.Application.Errors.Reservation;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Services;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MovieReservation.Tests.Unit.Application.Services;

public class ReservationServiceTests
{
	private readonly ReservationService _sut;
	private readonly IShowtimeRepository _showtimeRepositoryStub;
	private readonly IRepositoryBase<Reservation> _reservationRepositoryStub;
    private readonly IRepositoryBase<Seat> _seatRepositoryStub;
    private readonly IDbContext _unitOfWorkStub;
	private readonly ILogger<ReservationService> _loggerStub;

	private static readonly Guid testId = Guid.NewGuid();
	private static readonly CreateReservationDto testRequest = new(testId, [testId]);

	public ReservationServiceTests()
	{
        _showtimeRepositoryStub = Substitute.For<IShowtimeRepository>();
		_reservationRepositoryStub = Substitute.For<IRepositoryBase<Reservation>>();
		_seatRepositoryStub = Substitute.For<IRepositoryBase<Seat>>();
		_unitOfWorkStub = Substitute.For<IDbContext>();
		_loggerStub = Substitute.For<ILogger<ReservationService>>();

		_sut = new ReservationService(_showtimeRepositoryStub, _reservationRepositoryStub, _seatRepositoryStub, _unitOfWorkStub, _loggerStub);
	}

	[Fact]
	public async Task Create_ReturnsNotFoundError_When_ShowtimeIsNotFound()
	{
        // Arrange
		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.ReturnsNull();

        // Act
        var actual = await _sut.Create(testId, testRequest, default);

		// Assert
		bool b = actual.IsFailed
			&& actual.GetFirstError() is NotFoundError err
			&& err.Message == AppErrors.NotFound(typeof(Showtime)).Message;

		Assert.True(b);
    }

	[Fact]
	public async Task Create_ReturnsNoSeatsError_When_RequestHasNoSeats()
	{
		// Arrange
		var emptySeatRequest = testRequest with { SeatsIds = Enumerable.Empty<Guid>() };

		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(new Showtime());

		// Act
		var actual = await _sut.Create(testId, emptySeatRequest, default);

		// Assert
		Assert.True(actual.IsFailed);
		Assert.IsType<NoSeatsError>(actual.GetFirstError());
	}

	[Theory]
	[InlineData(11)]
	[InlineData(20)]
	[InlineData(1000)]
	public async Task Create_ReturnsTooManySeatsError_When_RequestHasMoreThanTenSeats(int seatsCount)
	{
		// Arrange
		var manySeatsRequest = testRequest with { SeatsIds = Enumerable.Repeat(testId, seatsCount) };

		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(new Showtime());

		// Act
		var actual = await _sut.Create(testId, manySeatsRequest, default);

		// Assert
		Assert.True(actual.IsFailed);
        Assert.IsType<TooManySeatsError>(actual.GetFirstError());
	}

	[Fact]
	public async Task Create_ReturnsNotFoundError_When_SeatIsNotFound()
	{
		// Arrange
		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(new Showtime());

		_seatRepositoryStub.GetByIdAsync(testRequest.SeatsIds.First(), default, Arg.Any<Expression<Func<Seat, object>>>())
			.ReturnsNull();

        // Act
        var actual = await _sut.Create(testId, testRequest, default);

        // Assert
        bool b = actual.IsFailed
            && actual.GetFirstError() is NotFoundError err
            && err.Message == AppErrors.NotFound(typeof(Seat)).Message;

        Assert.True(b);
    }

	[Fact]
	public async Task Create_ReturnsWrongHallError_When_SeatIsNotInShowtimesHall()
	{
        // Arrange
		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(new Showtime() { HallId = testId });

		_seatRepositoryStub.GetByIdAsync(testRequest.SeatsIds.First(), default, Arg.Any<Expression<Func<Seat, object>>>())
			.Returns(new Seat(1) { HallId = Guid.NewGuid() });

        // Act
        var actual = await _sut.Create(testId, testRequest, default);

		// Assert
		Assert.True(actual.IsFailed);
		Assert.IsType<WrongHallError>(actual.GetFirstError());
    }

	[Fact]
	public async Task Create_ReturnsAlreadyReservedError_When_SeatIsAlreadyReserved()
    {
        // Arrange
		var testShowtime = new Showtime() { HallId = testId };
		var testSeat = new Seat(1)
		{
			HallId = testShowtime.HallId,
			SeatReservations =
			{
				new() { ShowtimeId = Guid.NewGuid() },
				new() { ShowtimeId = testRequest.ShowtimeId },
				new() { ShowtimeId = Guid.NewGuid() }
			}
        };
	
		_showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(testShowtime);

		_seatRepositoryStub.GetByIdAsync(testRequest.SeatsIds.First(), default, Arg.Any<Expression<Func<Seat, object>>>())
			.Returns(testSeat);

        // Act
        var actual = await _sut.Create(testId, testRequest, default);

		// Assert
		Assert.True(actual.IsFailed);
		Assert.IsType<AlreadyReservedError>(actual.GetFirstError());
    }

	[Fact]
	public async Task Create_ReturnsSuccess()
	{
        // Arrange
        var testShowtime = new Showtime() { HallId = testId };
		
        _showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
			.Returns(testShowtime);

        _seatRepositoryStub.GetByIdAsync(testRequest.SeatsIds.First(), default, Arg.Any<Expression<Func<Seat, object>>>())
            .Returns(new Seat(1) { HallId = testShowtime.HallId });

		// Act
		var actual = await _sut.Create(testId, testRequest, default);

		// Assert
		Assert.True(actual.IsSuccess);
    }

    [Fact]
    public async Task Create_CallsAddAndSaveChanges()
    {
        // Arrange
        var testShowtime = new Showtime() { HallId = testId };
        var testSeat = new Seat(1) { HallId = testShowtime.HallId };

        _showtimeRepositoryStub.GetByIdAsync(testRequest.ShowtimeId, default)
            .Returns(testShowtime);

        _seatRepositoryStub.GetByIdAsync(testRequest.SeatsIds.First(), default, Arg.Any<Expression<Func<Seat, object>>>())
            .Returns(testSeat);

        // Act
        await _sut.Create(testId, testRequest, default);

		// Assert
		_reservationRepositoryStub.Received(1)
			.Add(Arg.Is<Reservation>(r =>
				r.SeatReservations.First().Showtime == testShowtime
				&& r.SeatReservations.First().Seat == testSeat));

        await _unitOfWorkStub.Received(1)
            .SaveChangesAsync(default);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundError_When_ReservationIsNotFound()
    {
        // Arrange
        _reservationRepositoryStub.GetByIdAsync(testId, default, Arg.Any<Expression<Func<Reservation, object>>>())
            .ReturnsNull();

        // Act
        var actual = await _sut.Delete(testId, testId, default);

        // Assert
        bool b = actual.IsFailed
            && actual.GetFirstError() is NotFoundError err
            && err.Message == AppErrors.NotFound(typeof(Reservation)).Message;

        Assert.True(b);
    }

    [Fact]
	public async Task Delete_ReturnsForbidError_When_UserIdsDontMatch()
	{
		// Arrange
		_reservationRepositoryStub.GetByIdAsync(testId, default, Arg.Any<Expression<Func<Reservation, object>>>())
			.Returns(new Reservation() { UserId = Guid.NewGuid() });

		// Act
		var actual = await _sut.Delete(testId, testId, default);

		// Assert
		Assert.True(actual.IsFailed);
        Assert.IsType<ForbidError>(actual.GetFirstError());
    }

	[Theory]
	[InlineData(2)]
	[InlineData(8)]
	[InlineData(12)]
	public async Task Delete_ReturnsTooLateToCancelError_When_LessTwelveHoursTillShowtime(int hours)
	{
		// Arrange
		Reservation testReservation = new()
		{
			UserId = testId,
			Showtime = new Showtime() { Timestamp = DateTime.UtcNow.AddHours(hours) }
		};

		_reservationRepositoryStub.GetByIdAsync(testId, default, Arg.Any<Expression<Func<Reservation, object>>>())
			.Returns(testReservation);

		// Act
		var actual = await _sut.Delete(testId, testId, default);

		// Assert
		Assert.True(actual.IsFailed);
        Assert.IsType<TooLateToCancelError>(actual.GetFirstError());
    }

    [Fact]
	public async Task Delete_ReturnsSuccess()
	{
		// Arrange
		Reservation testReservation = new()
		{
			UserId = testId,
			Showtime = new Showtime() { Timestamp = DateTime.UtcNow.AddDays(1) }
		};

		_reservationRepositoryStub.GetByIdAsync(testId, default, Arg.Any<Expression<Func<Reservation, object>>>())
			.Returns(testReservation);

		// Act
		var actual = await _sut.Delete(testId, testId, default);

		// Assert
		_reservationRepositoryStub.Received(1)
            .Delete(Arg.Is<Reservation>(r => r.UserId == testId));

		await _unitOfWorkStub.Received(1)
			.SaveChangesAsync(default);

		Assert.True(actual.IsSuccess);
	}

	[Fact]
	public async Task Delete_CallsDeleteAndSaveChanges()
    {
        // Arrange
        Reservation testReservation = new()
        {
			Id = testId,
            UserId = testId,
            Showtime = new Showtime() { Timestamp = DateTime.UtcNow.AddDays(1) }
        };

		_reservationRepositoryStub.GetByIdAsync(testId, default, Arg.Any<Expression<Func<Reservation, object>>>())
			.Returns(testReservation);

		// Act
		await _sut.Delete(testId, testId, default);

        // Assert
        _reservationRepositoryStub.Received(1)
            .Delete(Arg.Is<Reservation>(r => r.Id == testId));

        await _unitOfWorkStub.Received(1)
            .SaveChangesAsync(default);
    }
}