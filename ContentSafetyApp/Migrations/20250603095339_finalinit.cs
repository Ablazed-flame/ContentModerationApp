using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentModerationApp.Migrations
{
    /// <inheritdoc />
    public partial class finalinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ContentSubmissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentSubmissions_UserId",
                table: "ContentSubmissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_ContentSubmissions_UserId",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContentSubmissions");
        }
    }
}
