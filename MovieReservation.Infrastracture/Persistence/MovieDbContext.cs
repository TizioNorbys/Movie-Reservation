using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Infrastracture.Persistence;

public class MovieDbContext : IdentityDbContext<User, Role, Guid>, IDbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options)
		: base(options)
	{
	}

    public DbSet<Hall> Halls { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatReservation> SeatReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);

        builder.HasDbFunction(typeof(MovieDbContext).GetMethod(nameof(Levenshtein), BindingFlags.Public | BindingFlags.Static)!)
            .HasName("levenshtein");
    }

    public static int Levenshtein(string compareTo, string value)
    {
        throw new NotSupportedException();
    }
}