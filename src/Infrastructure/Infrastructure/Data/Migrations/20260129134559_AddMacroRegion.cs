using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMacroRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "macro_region_id",
                table: "clients",
                type: "uuid",
                nullable: true);

            // Use IF NOT EXISTS to avoid failing when the table already exists (seed created it previously)
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS macro_regions (
                    id uuid NOT NULL,
                    name text NOT NULL,
                    is_deleted boolean NOT NULL,
                    deleted_at timestamp with time zone,
                    CONSTRAINT ""PK_macro_regions"" PRIMARY KEY (id)
                );
            ");

            migrationBuilder.CreateIndex(
                name: "IX_clients_macro_region_id",
                table: "clients",
                column: "macro_region_id");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_macro_regions_macro_region_id",
                table: "clients",
                column: "macro_region_id",
                principalTable: "macro_regions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clients_macro_regions_macro_region_id",
                table: "clients");

            migrationBuilder.DropTable(
                name: "macro_regions");

            migrationBuilder.DropIndex(
                name: "IX_clients_macro_region_id",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "macro_region_id",
                table: "clients");
        }
    }
}
