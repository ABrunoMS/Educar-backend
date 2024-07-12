using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "game_id",
                table: "contract",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "game",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    lore = table.Column<string>(type: "text", nullable: false),
                    purpose = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contract_game_id",
                table: "contract",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "FK_contract_game_game_id",
                table: "contract",
                column: "game_id",
                principalTable: "game",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contract_game_game_id",
                table: "contract");

            migrationBuilder.DropTable(
                name: "game");

            migrationBuilder.DropIndex(
                name: "IX_contract_game_id",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "game_id",
                table: "contract");
        }
    }
}
