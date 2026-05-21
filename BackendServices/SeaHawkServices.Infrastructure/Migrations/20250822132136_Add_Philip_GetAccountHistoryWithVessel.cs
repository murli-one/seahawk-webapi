using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Philip_GetAccountHistoryWithVessel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
/****** Object:  StoredProcedure [dbo].[Philip_GetAccountHistoryWithVessel] ******/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'[dbo].[Philip_GetAccountHistoryWithVessel]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[Philip_GetAccountHistoryWithVessel];
GO
", suppressTransaction: true);

            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[Philip_GetAccountHistoryWithVessel]
    @UserId NVARCHAR(MAX) = NULL,
    @FuelType NVARCHAR(MAX) = NULL,
    @Specification NVARCHAR(50) = NULL,
    @FromDate NVARCHAR(MAX) = NULL,
    @ToDate NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Specification = 'ALL')
    BEGIN
        SELECT 
            VD.VesselName,
            AR.PortBunkered,
            AR.DateBunkered,
            LTRIM(RTRIM(AR.Specification)) AS Specification,
            AR.FuelType,
            AR.DateReceived,
            AR.SampleNumber,
            AR.Comment
        FROM AnalysisResults AR
        INNER JOIN VesselDetails VD ON AR.VesselDetailsId = VD.Id
        INNER JOIN VesselUsers VU ON VD.Id = VU.VesselDetailsId
        INNER JOIN AspNetUsers U ON VU.UserId = U.Id
        WHERE 
            U.Id = @UserId
            AND AR.FuelType LIKE '%' + ISNULL(@FuelType,'') + '%'
            AND CONVERT(DATE, AR.DateReceived) >= CONVERT(DATE, @FromDate)
            AND CONVERT(DATE, AR.DateReceived) <= CONVERT(DATE, @ToDate)
        ORDER BY AR.DateReceived DESC;
    END
    ELSE IF (@Specification IS NOT NULL AND @Specification <> 'ALL')
    BEGIN
        SELECT 
            VD.VesselName,
            AR.PortBunkered,
            AR.DateBunkered,
            LTRIM(RTRIM(AR.Specification)) AS Specification,
            AR.FuelType,
            AR.DateReceived,
            AR.SampleNumber,
            AR.Comment
        FROM AnalysisResults AR
        INNER JOIN VesselDetails VD ON AR.VesselDetailsId = VD.Id
        INNER JOIN VesselUsers VU ON VD.Id = VU.VesselDetailsId
        INNER JOIN AspNetUsers U ON VU.UserId = U.Id
        WHERE 
            U.Id = @UserId
            AND AR.Specification = @Specification
            AND AR.FuelType LIKE '%' + ISNULL(@FuelType,'') + '%'
            AND CONVERT(DATE, AR.DateReceived) >= CONVERT(DATE, @FromDate)
            AND CONVERT(DATE, AR.DateReceived) <= CONVERT(DATE, @ToDate)
        ORDER BY AR.DateReceived DESC;
    END
    ELSE
    BEGIN
        SELECT 
            VD.VesselName,
            AR.PortBunkered,
            AR.DateBunkered,
            LTRIM(RTRIM(AR.Specification)) AS Specification,
            AR.FuelType,
            AR.DateReceived,
            AR.SampleNumber,
            AR.Comment
        FROM AnalysisResults AR
        INNER JOIN VesselDetails VD ON AR.VesselDetailsId = VD.Id
        INNER JOIN VesselUsers VU ON VD.Id = VU.VesselDetailsId
        INNER JOIN AspNetUsers U ON VU.UserId = U.Id
        WHERE 
            U.Id = @UserId
            AND AR.FuelType LIKE '%' + ISNULL(@FuelType,'') + '%'
            AND CONVERT(DATE, AR.DateReceived) >= CONVERT(DATE, @FromDate)
            AND CONVERT(DATE, AR.DateReceived) <= CONVERT(DATE, @ToDate)
        ORDER BY AR.DateReceived DESC;
    END
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
