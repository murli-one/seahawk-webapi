using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fillPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"
            INSERT INTO AspNetUsers
            (
                Id,
                UserName,
                Discriminator,
                NormalizedUserName,
                Email,
                NormalizedEmail,
                EmailConfirmed, 
                PasswordHash,
                SecurityStamp,
                ConcurrencyStamp,
                LockoutEnabled,
                AccessFailedCount,
                PhoneNumberConfirmed,
                TwoFactorEnabled
            )
            SELECT 
                CAST(u.UserId AS nvarchar(450)) AS Id,
                u.UserName,
                'ApplicationUser',
                UPPER(u.UserName),
                m.Email,
                UPPER(m.Email),
                0,
                REPLACE(u.UserName, ' ', '') + '#',
                NEWID(),
                NEWID(),
                CASE WHEN m.IsLockedOut = 1 THEN 1 ELSE 0 END,
                0,
                0,
                0
            FROM SeaHawkDNNDb.dbo.aspnet_Users u
            JOIN SeaHawkDNNDb.dbo.aspnet_Membership m ON u.UserId = m.UserId
            WHERE NOT EXISTS (
                SELECT 1 
                FROM AspNetUsers newu 
                WHERE newu.Id = CAST(u.UserId AS nvarchar(450))
            );
        ";
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
