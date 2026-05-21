using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserloginHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VesselDetailId",
                table: "RequestHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLoginHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoginTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_VesselDetailId",
                table: "RequestHistory",
                column: "VesselDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_UserId",
                table: "UserLoginHistory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistory_VesselDetail_VesselDetailId",
                table: "RequestHistory",
                column: "VesselDetailId",
                principalTable: "VesselDetail",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistory_VesselDetail_VesselDetailId",
                table: "RequestHistory");

            migrationBuilder.DropTable(
                name: "UserLoginHistory");

            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_VesselDetailId",
                table: "RequestHistory");

            migrationBuilder.DropColumn(
                name: "VesselDetailId",
                table: "RequestHistory");
        }
    }
}
