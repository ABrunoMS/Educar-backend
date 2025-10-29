﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountSchoolManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_schools_school_id",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_school_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "school_id",
                table: "accounts");

            migrationBuilder.CreateTable(
                name: "account_schools",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_schools", x => new { x.account_id, x.school_id });
                    table.ForeignKey(
                        name: "FK_account_schools_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_schools_schools_school_id",
                        column: x => x.school_id,
                        principalTable: "schools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_schools_school_id",
                table: "account_schools",
                column: "school_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_schools");

            migrationBuilder.AddColumn<Guid>(
                name: "school_id",
                table: "accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounts_school_id",
                table: "accounts",
                column: "school_id");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_schools_school_id",
                table: "accounts",
                column: "school_id",
                principalTable: "schools",
                principalColumn: "id");
        }
    }
}