using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProficiencyGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "proficiency_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proficiency_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_proficiency_groups",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_proficiency_groups", x => new { x.game_id, x.proficiency_group_id });
                    table.ForeignKey(
                        name: "FK_game_proficiency_groups_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_proficiency_groups_proficiency_groups_proficiency_grou~",
                        column: x => x.proficiency_group_id,
                        principalTable: "proficiency_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proficiency_group_proficiencies",
                columns: table => new
                {
                    proficiency_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proficiency_group_proficiencies", x => new { x.proficiency_group_id, x.proficiency_id });
                    table.ForeignKey(
                        name: "FK_proficiency_group_proficiencies_proficiencies_proficiency_id",
                        column: x => x.proficiency_id,
                        principalTable: "proficiencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_proficiency_group_proficiencies_proficiency_groups_proficie~",
                        column: x => x.proficiency_group_id,
                        principalTable: "proficiency_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_proficiency_groups_proficiency_group_id",
                table: "game_proficiency_groups",
                column: "proficiency_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_proficiency_group_proficiencies_proficiency_id",
                table: "proficiency_group_proficiencies",
                column: "proficiency_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_proficiency_groups");

            migrationBuilder.DropTable(
                name: "proficiency_group_proficiencies");

            migrationBuilder.DropTable(
                name: "proficiency_groups");
        }
    }
}
