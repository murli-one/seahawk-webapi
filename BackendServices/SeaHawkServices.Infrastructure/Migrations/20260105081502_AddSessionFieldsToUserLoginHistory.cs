using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionFieldsToUserLoginHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserLoginHistory",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutTimeUtc",
                table: "UserLoginHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoutType",
                table: "UserLoginHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "UserLoginHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoutTimeUtc",
                table: "UserLoginHistory");

            migrationBuilder.DropColumn(
                name: "LogoutType",
                table: "UserLoginHistory");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserLoginHistory");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserLoginHistory",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }
    }
}
