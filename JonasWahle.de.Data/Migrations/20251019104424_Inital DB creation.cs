using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonasWahle.de.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitalDBcreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JW_DownloadItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    FontColor = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Platforms = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DownloadUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    GitHubUrl = table.Column<string>(type: "TEXT", nullable: true),
                    InsertTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JW_DownloadItem", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JW_DownloadItem");
        }
    }
}
