using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContentIdAndProductIdToQuestAsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quests_contents_content_id",
                table: "quests");

            migrationBuilder.DropForeignKey(
                name: "FK_quests_products_product_id",
                table: "quests");

            migrationBuilder.AlterColumn<Guid>(
                name: "product_id",
                table: "quests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "content_id",
                table: "quests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_quests_contents_content_id",
                table: "quests",
                column: "content_id",
                principalTable: "contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_quests_products_product_id",
                table: "quests",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quests_contents_content_id",
                table: "quests");

            migrationBuilder.DropForeignKey(
                name: "FK_quests_products_product_id",
                table: "quests");

            migrationBuilder.AlterColumn<Guid>(
                name: "product_id",
                table: "quests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "content_id",
                table: "quests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_quests_contents_content_id",
                table: "quests",
                column: "content_id",
                principalTable: "contents",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_quests_products_product_id",
                table: "quests",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");
        }
    }
}
