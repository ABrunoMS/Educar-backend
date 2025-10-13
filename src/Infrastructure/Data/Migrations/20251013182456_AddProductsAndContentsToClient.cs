using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsAndContentsToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "selected_contents",
                table: "clients",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "selected_products",
                table: "clients",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.CreateTable(
                name: "subsecretarias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsecretarias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regionals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    subsecretaria_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regionals", x => x.id);
                    table.ForeignKey(
                        name: "FK_regionals_subsecretarias_subsecretaria_id",
                        column: x => x.subsecretaria_id,
                        principalTable: "subsecretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_regionals_subsecretaria_id",
                table: "regionals",
                column: "subsecretaria_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "regionals");

            migrationBuilder.DropTable(
                name: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "selected_contents",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "selected_products",
                table: "clients");
        }
    }
}
