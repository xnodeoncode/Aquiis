using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Professional.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000015"),
                column: "RequiresValue",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000016"),
                column: "RequiresValue",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000023"),
                column: "RequiresValue",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000015"),
                column: "RequiresValue",
                value: true);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000016"),
                column: "RequiresValue",
                value: true);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000023"),
                column: "RequiresValue",
                value: true);
        }
    }
}
