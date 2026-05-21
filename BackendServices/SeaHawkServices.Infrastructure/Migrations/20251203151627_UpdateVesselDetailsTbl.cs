using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVesselDetailsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"UPDATE AnalysisResults 
            SET VesselDetailsId = NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "VesselUsers");

            migrationBuilder.DropTable(
                name: "VesselDetails");



            migrationBuilder.CreateTable(
                name: "VesselDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HFO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diesel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Charterer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Built = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dwt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Draft = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Propulsion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FuelSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HFOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByFax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByEMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByTelex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FOReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExVesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselDetail_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VesselUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VesselDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselUser_VesselDetail_VesselDetailId",
                        column: x => x.VesselDetailId,
                        principalTable: "VesselDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VesselDetail_CompanyId",
                table: "VesselDetail",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselUser_UserId",
                table: "VesselUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselUser_VesselDetailId",
                table: "VesselUser",
                column: "VesselDetailId");

            migrationBuilder.RenameColumn(
    name: "VesselDetailsId",
    table: "AnalysisResults",
    newName: "VesselDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResults_VesselDetailsId",
                table: "AnalysisResults",
                newName: "IX_AnalysisResults_VesselDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_VesselDetail_VesselDetailId",
                table: "AnalysisResults",
                column: "VesselDetailId",
                principalTable: "VesselDetail",
                principalColumn: "Id");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_VesselDetail_VesselDetailId",
                table: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "VesselUser");

            migrationBuilder.DropTable(
                name: "VesselDetail");

            migrationBuilder.RenameColumn(
                name: "VesselDetailId",
                table: "AnalysisResults",
                newName: "VesselDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_AnalysisResults_VesselDetailId",
                table: "AnalysisResults",
                newName: "IX_AnalysisResults_VesselDetailsId");

            migrationBuilder.CreateTable(
                name: "VesselDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    BillTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Built = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByEMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByFax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByTelex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Charterer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diesel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Draft = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dwt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExVesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FOReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FuelSystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HFO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HFOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFOGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Propulsion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselDetails_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VesselUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VesselDetailsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselUsers_VesselDetails_VesselDetailsId",
                        column: x => x.VesselDetailsId,
                        principalTable: "VesselDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VesselDetails_CompanyId",
                table: "VesselDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselUsers_UserId",
                table: "VesselUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselUsers_VesselDetailsId",
                table: "VesselUsers",
                column: "VesselDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults",
                column: "VesselDetailsId",
                principalTable: "VesselDetails",
                principalColumn: "Id");
        }
    }
}
