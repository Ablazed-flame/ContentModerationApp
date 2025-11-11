using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentModerationApp.Migrations
{
    /// <inheritdoc />
    public partial class alothappened : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "FlagReasons",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "ModerationSummary",
                table: "ContentSubmissions");

            migrationBuilder.DropColumn(
                name: "TextContent",
                table: "ContentSubmissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContentSubmissions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ContentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentSubmissionId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentItems_ContentSubmissions_ContentSubmissionId",
                        column: x => x.ContentSubmissionId,
                        principalTable: "ContentSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentItemId = table.Column<int>(type: "int", nullable: false),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    ModerationSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlagReasons = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationResults_ContentItems_ContentItemId",
                        column: x => x.ContentItemId,
                        principalTable: "ContentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_ContentSubmissionId",
                table: "ContentItems",
                column: "ContentSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationResults_ContentItemId",
                table: "ModerationResults",
                column: "ContentItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions");

            migrationBuilder.DropTable(
                name: "ModerationResults");

            migrationBuilder.DropTable(
                name: "ContentItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContentSubmissions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FlagReasons",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationSummary",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextContent",
                table: "ContentSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentSubmissions_AspNetUsers_UserId",
                table: "ContentSubmissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
