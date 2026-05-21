using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompanyUserNewData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    //        migrationBuilder.Sql(@"
    //INSERT INTO [CompanyUser] (CompanyId, UserId)
    //SELECT
    //    c.Id AS CompanyId,
    //    u.Id AS UserId
    //FROM [SS].[dbo].[CompanyUsers] cu
    //OUTER APPLY (
    //    SELECT TOP 1 Id
    //    FROM [Company]
    //    WHERE CompanyName = cu.CompanyName
    //    ORDER BY Id
    //) c
    //OUTER APPLY (
    //    SELECT TOP 1 Id
    //    FROM [AspNetUsers]
    //    WHERE UserName = cu.UserName
    //    ORDER BY Id
    //) u
    //WHERE c.Id IS NOT NULL AND u.Id IS NOT NULL;
    //");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM [CompanyUser]
        WHERE EXISTS (
            SELECT 1 FROM [Company] c
            JOIN [AspNetUsers] u ON 1 = 1
            JOIN [SS].[dbo].[CompanyUsers] cu
                ON cu.CompanyName = c.CompanyName AND cu.UserName = u.UserName
            WHERE [CompanyUser].CompanyId = c.Id AND [CompanyUser].UserId = u.Id
        );
    ");
        }
    }
}
