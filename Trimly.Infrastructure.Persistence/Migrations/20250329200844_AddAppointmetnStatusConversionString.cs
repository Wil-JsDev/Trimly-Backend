using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmetnStatusConversionString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppointmentStatus",
                table: "Appointment",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AppointmentStatus",
                table: "Appointment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
