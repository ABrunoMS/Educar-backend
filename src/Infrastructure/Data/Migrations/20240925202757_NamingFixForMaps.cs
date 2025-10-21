using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NamingFixForMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "refence3d",
                table: "maps",
                newName: "reference3d");

            migrationBuilder.RenameColumn(
                name: "refence2d",
                table: "maps",
                newName: "reference2d");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reference3d",
                table: "maps",
                newName: "refence3d");

            migrationBuilder.RenameColumn(
                name: "reference2d",
                table: "maps",
                newName: "refence2d");
        }
    }
}
