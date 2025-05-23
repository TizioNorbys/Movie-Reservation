using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Infrastracture.Persistence.EntitiesConfiguration;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.Property(r => r.Code)
            .HasColumnType("char(20)");

        builder.HasMany(r => r.Seats)
            .WithMany(s => s.Reservation)
            .UsingEntity<SeatReservation>(
                l => l.HasOne(sr => sr.Seat).WithMany(s => s.SeatReservations).HasForeignKey(sr => sr.SeatId).OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(sr => sr.Reservation).WithMany(r => r.SeatReservations).HasForeignKey(sr => sr.ReservationId).OnDelete(DeleteBehavior.Cascade),
                j =>
                    {
                        j.HasKey(sr => new { sr.SeatId, sr.ShowtimeId });

                        j.Property(sr => sr.ReservedAt)
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("(UTC_TIMESTAMP())");

                        j.HasOne(sr => sr.Showtime).WithMany(s => s.SeatReservations).HasForeignKey(sr => sr.ShowtimeId).OnDelete(DeleteBehavior.Cascade);
                    });
    }
}