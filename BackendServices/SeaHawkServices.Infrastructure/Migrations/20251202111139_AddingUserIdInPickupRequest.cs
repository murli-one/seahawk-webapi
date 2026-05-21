using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserIdInPickupRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "PickupRequest",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequest_ApplicationUserId",
                table: "PickupRequest",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickupRequest_AspNetUsers_ApplicationUserId",
                table: "PickupRequest",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickupRequest_AspNetUsers_ApplicationUserId",
                table: "PickupRequest");

            migrationBuilder.DropIndex(
                name: "IX_PickupRequest_ApplicationUserId",
                table: "PickupRequest");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "PickupRequest");
        }
    }
}
