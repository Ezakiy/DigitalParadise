using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumDigitalParadise.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfileImageUrlAddedToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Notifications");
        }
    }
}
