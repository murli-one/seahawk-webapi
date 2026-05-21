using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Philip_GetLiveData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE [dbo].[Philip_GetLiveData]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        AR.SampleNumber,
        LTRIM(RTRIM(AR.Specification)) AS [Specification],
        VD.VesselName AS [VesselName],
        AR.SampleLocation,
        REPLACE(CONVERT(NVARCHAR(12), AR.SampleSentOn, 106), ' ', '-') AS [SampleSentOn],
        AR.PortBunkered AS [Port],
        AR.FuelType,
        CAST(AR.Density AS DECIMAL(10,1))   AS [Density],
        CAST(AR.CstAt50 AS DECIMAL(10,2))   AS [CstAt50],
        CAST(AR.Sulphur AS DECIMAL(10,2))   AS [Sulphur],
        CAST(AR.PourPointSummStd AS DECIMAL(10,0)) AS [PourPointSummStd],
        CAST(AR.FlashPoint AS DECIMAL(10,1)) AS [FlashPoint],
        CAST(AR.Water AS DECIMAL(10,2))     AS [Water],
        CAST(AR.MCR AS DECIMAL(10,2))       AS [MCR],
        CAST(AR.Aluminum AS DECIMAL(10,0))  AS [Aluminum],
        CAST(AR.Silicon AS DECIMAL(10,0))   AS [Silicon],
        CAST(AR.AlSi AS DECIMAL(10,0))      AS [AlSi],
        CAST(AR.Ash AS DECIMAL(10,2))       AS [Ash],
        CAST(AR.Vanadium AS DECIMAL(10,0))  AS [Vanadium],
        CAST(AR.Sodium AS DECIMAL(10,0))    AS [Sodium],
        CAST(AR.TSP AS DECIMAL(10,2))       AS [TSP],
        CAST(AR.CCAI AS DECIMAL(10,0))      AS [CCAI],
        CAST(AR.Ca AS DECIMAL(10,0))        AS [Ca],
        CAST(AR.Zn AS DECIMAL(10,0))        AS [Zn],
        CAST(AR.P AS DECIMAL(10,0))         AS [P],
        CAST(AR.TotalAcid AS DECIMAL(10,2)) AS [TotalAcid],
        CAST(AR.CloudPoint AS DECIMAL(10,0)) AS [CloudPoint],
        CAST(AR.Cetane AS DECIMAL(10,1))    AS [Cetane],
        AR.Appearance,
        AR.FTIR,
        CAST(AR.NetCalVal AS DECIMAL(10,2)) AS [NetCalVal]
    FROM dbo.AnalysisResults AR
    JOIN dbo.VesselDetails VD ON AR.VesselDetailsId = VD.Id
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.CompanyUser CU
        JOIN dbo.Company C ON C.Id = CU.CompanyId
        WHERE CU.UserId = @UserId
          AND LTRIM(RTRIM(VD.[OWNER])) = LTRIM(RTRIM(C.CompanyName))
    )
      AND AR.Specification = 'In process'
      AND CAST(AR.DateReceived AS date) <= CAST(GETDATE() AS date)
    ORDER BY AR.DateReceived DESC;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
