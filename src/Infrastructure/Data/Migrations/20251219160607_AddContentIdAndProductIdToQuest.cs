using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContentIdAndProductIdToQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_clients_client_id",
                table: "accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "content_id",
                table: "quests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "product_id",
                table: "quests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "client_id",
                table: "accounts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_quests_content_id",
                table: "quests",
                column: "content_id");

            migrationBuilder.CreateIndex(
                name: "IX_quests_product_id",
                table: "quests",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_clients_client_id",
                table: "accounts",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_clients_client_id",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_quests_contents_content_id",
                table: "quests");

            migrationBuilder.DropForeignKey(
                name: "FK_quests_products_product_id",
                table: "quests");

            migrationBuilder.DropIndex(
                name: "IX_quests_content_id",
                table: "quests");

            migrationBuilder.DropIndex(
                name: "IX_quests_product_id",
                table: "quests");

            migrationBuilder.DropColumn(
                name: "content_id",
                table: "quests");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "quests");

            migrationBuilder.AlterColumn<Guid>(
                name: "client_id",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_clients_client_id",
                table: "accounts",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
