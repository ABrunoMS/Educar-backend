using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameNpcRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_npcs",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    npc_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_npcs", x => new { x.game_id, x.npc_id });
                    table.ForeignKey(
                        name: "FK_game_npcs_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_npcs_npcs_npc_id",
                        column: x => x.npc_id,
                        principalTable: "npcs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_npcs_npc_id",
                table: "game_npcs",
                column: "npc_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_npcs");
        }
    }
}
