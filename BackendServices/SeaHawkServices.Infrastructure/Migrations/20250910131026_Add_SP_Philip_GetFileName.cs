using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_SP_Philip_GetFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[Philip_GetFileName]
                    @SampleNumber NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT FileName
                    FROM dbo.SampleNumberNames
                    WHERE SampleNumber = @SampleNumber;
                END
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
