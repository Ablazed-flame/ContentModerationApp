using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentModerationApp.Migrations
{
    /// <inheritdoc />
    public partial class j : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TextContent",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdminOverriddenAt",
                table: "ContentSubmissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AdminOverrideFlag",
                table: "ContentSubmissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminOverrideNote",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOverriddenAt",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "AdminOverrideFlag",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "AdminOverrideNote",
                table: "ContentSubmissions");

            migrationBuilder.AlterColumn<string>(
                name: "TextContent",
                table: "ContentSubmissions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
