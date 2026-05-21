using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalysisResultTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE ar
SET ar.VesselDetailId = ar.VDId
FROM AnalysisResults ar
INNER JOIN VesselDetail vd ON vd.Id = ar.VDId
WHERE ar.VDId IS NOT NULL;
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
