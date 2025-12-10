using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubsecretaryAndRegionaisFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nome",
                table: "subsecretarias",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "regionals",
                newName: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "subsecretarias",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "regionals",
                newName: "nome");
        }
    }
}
