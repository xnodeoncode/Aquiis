using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTestEntityAndPropertyTestField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestField",
                table: "Properties",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    TestName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TestDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TestDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TestNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TestFlag = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestEntities_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestEntities_IsDeleted",
                table: "TestEntities",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestEntities_OrganizationId",
                table: "TestEntities",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEntities_TestName",
                table: "TestEntities",
                column: "TestName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestEntities");

            migrationBuilder.DropColumn(
                name: "TestField",
                table: "Properties");
        }
    }
}
