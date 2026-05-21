using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreatedUserId_SampleCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                table: "SampleCollection");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "SampleCollection",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                table: "SampleCollection",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                table: "SampleCollection");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "SampleCollection",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                table: "SampleCollection",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
