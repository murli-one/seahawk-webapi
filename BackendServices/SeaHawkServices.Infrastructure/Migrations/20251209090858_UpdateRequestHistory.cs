using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiDispatchReference",
                table: "RequestHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Courier",
                table: "RequestHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "RequestHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldStatus",
                table: "RequestHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "RequestHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiDispatchReference",
                table: "RequestHistory");

            migrationBuilder.DropColumn(
                name: "Courier",
                table: "RequestHistory");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "RequestHistory");

            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "RequestHistory");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "RequestHistory");
        }
    }
}
