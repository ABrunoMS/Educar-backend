using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToSubsecretariaAndUpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_regionals_subsecretarias_subsecretaria_id",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "regional",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "secretary",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "sub_secretary",
                table: "clients");

            migrationBuilder.AddColumn<Guid>(
                name: "client_id",
                table: "subsecretarias",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created",
                table: "subsecretarias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "subsecretarias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "subsecretarias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "subsecretarias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_modified",
                table: "subsecretarias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "subsecretarias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "secretary_id",
                table: "subsecretarias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created",
                table: "regionals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "regionals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "regionals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "regionals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_modified",
                table: "regionals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "regionals",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_client_id",
                table: "subsecretarias",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_secretary_id",
                table: "subsecretarias",
                column: "secretary_id");

            migrationBuilder.AddForeignKey(
                name: "FK_regionals_subsecretarias_subsecretaria_id",
                table: "regionals",
                column: "subsecretaria_id",
                principalTable: "subsecretarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subsecretarias_clients_client_id",
                table: "subsecretarias",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subsecretarias_secretaries_secretary_id",
                table: "subsecretarias",
                column: "secretary_id",
                principalTable: "secretaries",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_regionals_subsecretarias_subsecretaria_id",
                table: "regionals");

            migrationBuilder.DropForeignKey(
                name: "FK_subsecretarias_clients_client_id",
                table: "subsecretarias");

            migrationBuilder.DropForeignKey(
                name: "FK_subsecretarias_secretaries_secretary_id",
                table: "subsecretarias");

            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_client_id",
                table: "subsecretarias");

            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_secretary_id",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "created",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "secretary_id",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "created",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "regionals");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "regionals");

            migrationBuilder.AddColumn<string>(
                name: "regional",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secretary",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_secretary",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_regionals_subsecretarias_subsecretaria_id",
                table: "regionals",
                column: "subsecretaria_id",
                principalTable: "subsecretarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
