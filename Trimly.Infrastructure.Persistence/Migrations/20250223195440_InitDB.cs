using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredCompany",
                columns: table => new
                {
                    RegisteredCompaniesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RNC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkRegisteredCompany", x => x.RegisteredCompaniesId);
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    ReservationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkReservation", x => x.ReservationsId);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    RegisteredCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkReview", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FkReviewsRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    SchedulesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    OpeningTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsHoliday = table.Column<int>(type: "int", nullable: false),
                    RegisteredCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkSchedule", x => x.SchedulesId);
                    table.ForeignKey(
                        name: "FkSchedulesRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    RegisteredCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkService", x => x.ServicesId);
                    table.ForeignKey(
                        name: "FkServicesRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentStatus = table.Column<int>(type: "int", nullable: true),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkAppointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FkAppointmentReservation",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkAppointmentService",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "ServicesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ReservationId",
                table: "Appointment",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ServiceId",
                table: "Appointment",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_RegisteredCompanyId",
                table: "Review",
                column: "RegisteredCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_RegisteredCompanyId",
                table: "Schedule",
                column: "RegisteredCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_RegisteredCompanyId",
                table: "Service",
                column: "RegisteredCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "RegisteredCompany");
        }
    }
}
