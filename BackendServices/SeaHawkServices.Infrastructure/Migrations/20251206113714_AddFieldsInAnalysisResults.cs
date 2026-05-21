using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsInAnalysisResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VDId",
                table: "AnalysisResult");

            migrationBuilder.AddColumn<int>(
                name: "AnalysisResultStatus",
                table: "AnalysisResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AnalysisResult",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
    name: "SampleCollectionsId",
    table: "AnalysisResult",
    type: "int",
    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportLink",
                table: "AnalysisResult",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDate",
                table: "AnalysisResult",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultSummary",
                table: "AnalysisResult",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResult_CompanyId",
                table: "AnalysisResult",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResult_SampleCollectionsId",
                table: "AnalysisResult",
                column: "SampleCollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResult_Company_CompanyId",
                table: "AnalysisResult",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResult_SampleCollections_SampleCollectionsId",
                table: "AnalysisResult",
                column: "SampleCollectionsId",
                principalTable: "SampleCollections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResult_Company_CompanyId",
                table: "AnalysisResult");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResult_SampleCollections_SampleCollectionsId",
                table: "AnalysisResult");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisResult_CompanyId",
                table: "AnalysisResult");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisResult_SampleCollectionsId",
                table: "AnalysisResult");

            migrationBuilder.DropColumn(
                name: "AnalysisResultStatus",
                table: "AnalysisResult");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AnalysisResult");

            migrationBuilder.DropColumn(
                name: "ReportLink",
                table: "AnalysisResult");

            migrationBuilder.DropColumn(
                name: "ResultDate",
                table: "AnalysisResult");

            migrationBuilder.DropColumn(
                name: "ResultSummary",
                table: "AnalysisResult");

            migrationBuilder.RenameColumn(
                name: "SampleCollectionsId",
                table: "AnalysisResult",
                newName: "VDId");
        }
    }
}
