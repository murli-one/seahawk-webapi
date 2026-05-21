using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatingSamplingKitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FedExInvoiceUrl",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FedExLabelUrl",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FedExTrackingNumber",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FedExInvoiceUrl",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "FedExLabelUrl",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "FedExTrackingNumber",
                table: "SamplingKit");
        }
    }
}
