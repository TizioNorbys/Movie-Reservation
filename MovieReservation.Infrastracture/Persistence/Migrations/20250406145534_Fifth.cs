using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservation.Infrastracture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_Timestamp",
                table: "Showtimes",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Showtimes_Timestamp",
                table: "Showtimes");
        }
    }
}
