using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredCompany",
                columns: table => new
                {
                    RegisteredCompaniesId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RNC = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkRegisteredCompany", x => x.RegisteredCompaniesId);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    RegisteredCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkReview", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FkRRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    SchedulesId = table.Column<Guid>(type: "uuid", nullable: false),
                    Week = table.Column<string>(type: "text", nullable: false),
                    OpeningTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ClosingTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsHoliday = table.Column<string>(type: "text", nullable: false),
                    RegisteredCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkSchedule", x => x.SchedulesId);
                    table.ForeignKey(
                        name: "FkSRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServicesId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RegisteredCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkService", x => x.ServicesId);
                    table.ForeignKey(
                        name: "FkSSRegisteredCompany",
                        column: x => x.RegisteredCompanyId,
                        principalTable: "RegisteredCompany",
                        principalColumn: "RegisteredCompaniesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppointmentStatus = table.Column<int>(type: "integer", nullable: false),
                    ConfirmationCode = table.Column<string>(type: "text", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkAppointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FkService",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "ServicesId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Service");

            migrationBuilder.DropTable(
                name: "RegisteredCompany");
        }
    }
}
