using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSmtpSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmtpSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpHost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpPort = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnableSSL = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmtpSettings", x => x.Id);
                });
            migrationBuilder.InsertData(
       table: "SmtpSettings",
       columns: new[] { "SmtpHost", "SmtpPort", "Email", "Password", "EnableSSL", "UpdatedOn" },
       values: new object[]
       {
            "smtp.office365.com", 587, "info@seahawkservices.com", "10%Piper32!", true, DateTime.UtcNow
       });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmtpSettings");
        }
    }
}
