using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumDigitalParadise.Data.Migrations
{
    /// <inheritdoc />
    public partial class FollowingDate_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FollowingDate",
                table: "Followers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowingDate",
                table: "Followers");
        }
    }
}
