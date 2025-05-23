using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Infrastracture.Persistence.EntitiesConfiguration;

public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.Property(s => s.Timestamp)
            .HasColumnType("datetime");

        builder.HasIndex(s => s.Timestamp);
    }
}