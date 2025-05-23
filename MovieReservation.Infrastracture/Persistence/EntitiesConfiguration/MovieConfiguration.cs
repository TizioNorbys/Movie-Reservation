using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Infrastracture.Persistence.EntitiesConfiguration;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(m => m.Title)
            .HasMaxLength(50);

        builder.HasIndex(m => m.Title);
        builder.HasIndex(m => m.Rating);

        builder.Property(m => m.Description)
            .HasMaxLength(300);

        builder.Property(m => m.Plot)
            .HasMaxLength(3000);

        builder.Property(m => m.ReleaseYear)
            .HasColumnType("year");

        builder.Property(m => m.OriginalLanguage)
            .HasColumnType("varchar(20)");

        builder.Property(m => m.Country)
            .HasColumnType("varchar(30)");

        builder.Property(m => m.Rating)
            .HasColumnType("decimal(3, 1)");

        builder.HasMany(m => m.Actors)
            .WithMany(a => a.Movies)
            .UsingEntity(
                "MovieActor",
                l => l.HasOne(typeof(Actor)).WithMany().HasForeignKey("ActorId").OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(Movie)).WithMany().HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade));

        builder.HasMany(m => m.Genres)
            .WithMany(g => g.Movies)
            .UsingEntity(
                "MovieGenre",
                l => l.HasOne(typeof(Genre)).WithMany().HasForeignKey("GenreId").OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(Movie)).WithMany().HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade));
    }
}