using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "classes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "school_shift",
                table: "classes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "school_year",
                table: "classes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "school_shift",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "school_year",
                table: "classes");
        }
    }
}
