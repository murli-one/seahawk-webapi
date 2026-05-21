using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserIdInSamplingKit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "SamplingKit",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SamplingKit_ApplicationUserId",
                table: "SamplingKit",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SamplingKit_AspNetUsers_ApplicationUserId",
                table: "SamplingKit",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SamplingKit_AspNetUsers_ApplicationUserId",
                table: "SamplingKit");

            migrationBuilder.DropIndex(
                name: "IX_SamplingKit_ApplicationUserId",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "SamplingKit");
        }
    }
}
