using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionalIdToSchool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "regional_id",
                table: "schools",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_schools_regional_id",
                table: "schools",
                column: "regional_id");

            migrationBuilder.AddForeignKey(
                name: "FK_schools_regionals_regional_id",
                table: "schools",
                column: "regional_id",
                principalTable: "regionals",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schools_regionals_regional_id",
                table: "schools");

            migrationBuilder.DropIndex(
                name: "IX_schools_regional_id",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "regional_id",
                table: "schools");
        }
    }
}
