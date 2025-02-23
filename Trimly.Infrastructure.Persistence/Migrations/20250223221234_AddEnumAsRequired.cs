using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumAsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkAppointmentReservation",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FkAppointmentService",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FkReviewsRegisteredCompany",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FkSchedulesRegisteredCompany",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FkServicesRegisteredCompany",
                table: "Service");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Week",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IsHoliday",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AppointmentStatus",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FkReservation",
                table: "Appointment",
                column: "ReservationId",
                principalTable: "Reservation",
                principalColumn: "ReservationsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkService",
                table: "Appointment",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "ServicesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkRegisteredCompany",
                table: "Review",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkRegisteredCompany",
                table: "Schedule",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkRegisteredCompany",
                table: "Service",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkReservation",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FkService",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FkRegisteredCompany",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FkRegisteredCompany",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FkRegisteredCompany",
                table: "Service");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Service",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Week",
                table: "Schedule",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "IsHoliday",
                table: "Schedule",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentStatus",
                table: "Appointment",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FkAppointmentReservation",
                table: "Appointment",
                column: "ReservationId",
                principalTable: "Reservation",
                principalColumn: "ReservationsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkAppointmentService",
                table: "Appointment",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "ServicesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkReviewsRegisteredCompany",
                table: "Review",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkSchedulesRegisteredCompany",
                table: "Schedule",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkServicesRegisteredCompany",
                table: "Service",
                column: "RegisteredCompanyId",
                principalTable: "RegisteredCompany",
                principalColumn: "RegisteredCompaniesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
