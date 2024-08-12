using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quest_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    npc_type = table.Column<string>(type: "text", nullable: false),
                    npc_behaviour = table.Column<string>(type: "text", nullable: false),
                    quest_step_type = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_steps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quest_step_contents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quest_step_content_type = table.Column<string>(type: "text", nullable: false),
                    question_type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    expected_answers = table.Column<string>(type: "jsonb", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    quest_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_step_contents", x => x.id);
                    table.ForeignKey(
                        name: "FK_quest_step_contents_quest_steps_quest_step_id",
                        column: x => x.quest_step_id,
                        principalTable: "quest_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quest_step_items",
                columns: table => new
                {
                    quest_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_step_items", x => new { x.quest_step_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_quest_step_items_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quest_step_items_quest_steps_quest_step_id",
                        column: x => x.quest_step_id,
                        principalTable: "quest_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quest_step_medias",
                columns: table => new
                {
                    quest_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_step_medias", x => new { x.quest_step_id, x.media_id });
                    table.ForeignKey(
                        name: "FK_quest_step_medias_medias_media_id",
                        column: x => x.media_id,
                        principalTable: "medias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quest_step_medias_quest_steps_quest_step_id",
                        column: x => x.quest_step_id,
                        principalTable: "quest_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quest_step_npcs",
                columns: table => new
                {
                    quest_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    npc_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_step_npcs", x => new { x.quest_step_id, x.npc_id });
                    table.ForeignKey(
                        name: "FK_quest_step_npcs_npcs_npc_id",
                        column: x => x.npc_id,
                        principalTable: "npcs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quest_step_npcs_quest_steps_quest_step_id",
                        column: x => x.quest_step_id,
                        principalTable: "quest_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quest_step_contents_quest_step_id",
                table: "quest_step_contents",
                column: "quest_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_quest_step_items_item_id",
                table: "quest_step_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_quest_step_medias_media_id",
                table: "quest_step_medias",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "IX_quest_step_npcs_npc_id",
                table: "quest_step_npcs",
                column: "npc_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quest_step_contents");

            migrationBuilder.DropTable(
                name: "quest_step_items");

            migrationBuilder.DropTable(
                name: "quest_step_medias");

            migrationBuilder.DropTable(
                name: "quest_step_npcs");

            migrationBuilder.DropTable(
                name: "quest_steps");
        }
    }
}
