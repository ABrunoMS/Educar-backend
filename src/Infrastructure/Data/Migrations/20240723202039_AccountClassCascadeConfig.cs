using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AccountClassCascadeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_classes_accounts_account_id",
                table: "account_classes");

            migrationBuilder.DropForeignKey(
                name: "FK_account_classes_classes_class_id",
                table: "account_classes");

            migrationBuilder.AddForeignKey(
                name: "FK_account_classes_accounts_account_id",
                table: "account_classes",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_account_classes_classes_class_id",
                table: "account_classes",
                column: "class_id",
                principalTable: "classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_classes_accounts_account_id",
                table: "account_classes");

            migrationBuilder.DropForeignKey(
                name: "FK_account_classes_classes_class_id",
                table: "account_classes");

            migrationBuilder.AddForeignKey(
                name: "FK_account_classes_accounts_account_id",
                table: "account_classes",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_account_classes_classes_class_id",
                table: "account_classes",
                column: "class_id",
                principalTable: "classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
