﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceStatus",
                table: "Service",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceStatus",
                table: "Service");
        }
    }
}
