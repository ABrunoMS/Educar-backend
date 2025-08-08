using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "clients");

            migrationBuilder.RenameColumn(
                name: "last_modified_by",
                table: "clients",
                newName: "validity");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "clients",
                newName: "sub_secretary");

            migrationBuilder.AddColumn<string>(
                name: "contacts",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contract",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "implantation_date",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "partner",
                table: "clients",
                type: "text",
                nullable: true);

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
                name: "signature_date",
                table: "clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_accounts",
                table: "clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contacts",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "contract",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "implantation_date",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "partner",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "regional",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "secretary",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "signature_date",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "total_accounts",
                table: "clients");

            migrationBuilder.RenameColumn(
                name: "validity",
                table: "clients",
                newName: "last_modified_by");

            migrationBuilder.RenameColumn(
                name: "sub_secretary",
                table: "clients",
                newName: "created_by");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created",
                table: "clients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_modified",
                table: "clients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
