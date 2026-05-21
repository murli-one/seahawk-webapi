using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVDId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VDId",
                table: "AnalysisResults",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
      @"UPDATE AnalysisResults 
          SET VDId = VesselDetailsId 
          WHERE VesselDetailsId IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VDId",
                table: "AnalysisResults");
        }
    }
}
