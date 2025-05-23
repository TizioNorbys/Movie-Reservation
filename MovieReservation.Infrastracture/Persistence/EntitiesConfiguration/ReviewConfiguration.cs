using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Infrastracture.Persistence.EntitiesConfiguration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasIndex(r => r.Rating);

        builder.Property(r => r.Title)
            .HasMaxLength(100);

        builder.Property(r => r.Content)
            .HasMaxLength(10000);

        builder.Property(r => r.Timestamp)
            .HasColumnType("datetime");
    }
}