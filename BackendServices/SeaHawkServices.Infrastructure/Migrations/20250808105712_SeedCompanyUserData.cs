using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompanyUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            WITH Mapping AS (
                SELECT * FROM (VALUES
                    ('rflaeisz', 'F.LAEISZ GMBH'),
                    ('jdoe', 'Oceanic Lines Inc'),
                    ('asmith', 'Global Marine Ltd')
                ) AS M(UserName, CompanyName)
            )
            INSERT INTO CompanyUser (CompanyId, UserId)
            SELECT 
                C.Id,
                U.Id
            FROM Mapping M
            INNER JOIN Company C ON C.CompanyName = M.CompanyName
            INNER JOIN AspNetUsers U ON U.UserName = M.UserName
            WHERE NOT EXISTS (
                SELECT 1 
                FROM CompanyUser CU 
                WHERE CU.CompanyId = C.Id AND CU.UserId = U.Id
            );
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            WITH Mapping AS (
                SELECT * FROM (VALUES
                    ('rflaeisz', 'F.LAEISZ GMBH'),
                    ('jdoe', 'Oceanic Lines Inc'),
                    ('asmith', 'Global Marine Ltd')
                ) AS M(UserName, CompanyName)
            )
            DELETE FROM CompanyUser
            WHERE EXISTS (
                SELECT 1
                FROM Mapping M
                INNER JOIN Company C ON C.CompanyName = M.CompanyName
                INNER JOIN AspNetUsers U ON U.UserName = M.UserName
                WHERE CompanyUser.CompanyId = C.Id AND CompanyUser.UserId = U.Id
            );
        ");
        }
    }
}
