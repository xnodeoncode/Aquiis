using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RentalApplications_ProspectiveTenantId",
                table: "RentalApplications");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualMoveOutDate",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedMoveOutDate",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RenewalNumber",
                table: "Leases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TerminationNoticedOn",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerminationReason",
                table: "Leases",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_ProspectiveTenantId",
                table: "RentalApplications",
                column: "ProspectiveTenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RentalApplications_ProspectiveTenantId",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "ActualMoveOutDate",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "ExpectedMoveOutDate",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "RenewalNumber",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "TerminationNoticedOn",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "TerminationReason",
                table: "Leases");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_ProspectiveTenantId",
                table: "RentalApplications",
                column: "ProspectiveTenantId",
                unique: true);
        }
    }
}
