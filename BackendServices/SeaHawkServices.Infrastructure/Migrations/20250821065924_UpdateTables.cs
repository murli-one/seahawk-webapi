using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "IMONumber",
                table: "VesselUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "VesselUsers");

            migrationBuilder.DropColumn(
                name: "VesselName",
                table: "AnalysisResults");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VesselUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "VesselName",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TlxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Registry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Purifier",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Propulsion",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IMONumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GeneratorType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FuelSystem",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Filter",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FaxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FaxCountry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FaxArea",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ExVesselName",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Dwt",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Draft",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Diesel",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CommissionType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Charterer",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CallSign",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ByTelex",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ByFax",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ByEMail",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Built",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BillTo",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "VesselDetailsId",
                table: "AnalysisResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Specification",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SampleNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PortBunkered",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateReceived",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateBunkered",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AASS",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "APIGravity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ASSESS",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirwayNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AlSi",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlSiAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Aluminum",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ant",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Appearance",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppearanceAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ash",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AshAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Asphaltenes",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BSW",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Be",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BunkerReceipt",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BunkerTanker",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BurningTemp",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "C",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CCAI",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CCAIAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ca",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cd",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cetane",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CetaneAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientRefNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CloudPoint",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloudPointAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Co",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ColdFilterPluggingPoint",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment1",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment2",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment3",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment4",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment5",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment6",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Compatibility",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Confirmation",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CopperCorrosion",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cr",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CstAr40Ass",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CstAt100",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CstAt40",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CstAt50",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CstAt50Ass",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CstAt80",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cu",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Demulsification",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Density",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DensityAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselRangeOrganics",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistResLoss",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EP",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FTIR",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FTIRAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fe",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FlashPoint",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlashPointAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FreeGlycerin",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelMixWithFuelOnboard",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuelTypeID",
                table: "AnalysisResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossHeatComb",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "H",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "H2S",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "H2SAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Halogens",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Hg",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IBP",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initials",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp10cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp12cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp13cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp14cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp15cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp20cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InjTemp25cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LID",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Laboratory",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Li",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Lubricity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LubricityAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MCR",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MCR10",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MCRAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MPT100cst",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Mg",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MineralAcidity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "N",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NaK",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NaVRatio",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetCalVal",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetHeatComb",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetSpecificEnergy",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Neutrality",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ni",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffSpec1",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffSpec2",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffSpec3",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffSpec4",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "AnalysisResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OxidationStability",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OxidationStabilityAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "P",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticulateAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ParticulateContamination",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pb",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct10",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct20",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct30",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct40",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct50",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pct90",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Polymer",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PourPointAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PourPointSummStd",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PourPointWint",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityDiff",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SSQuantity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleLocation",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleMethod",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SampleReportDate",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleSentFrom",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SampleSentOn",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Se",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SealNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SealNumberMARPOL",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SealNumberRetained",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SealNumberSupplier",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sediment",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCharges",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Silicon",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sodium",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SodiumAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Stability",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StrongAcid",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sulphur",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SulphurAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SulphurPPM",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SulphurPPMAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierDensity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierSulfur",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierSulfurPPM",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierSulphur",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplierViscosity",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TSA",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TSE",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TSEAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TSP",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TankNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Technician",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAcid",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalAcidAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeposits",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGlycerin",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ULO",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ULOAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Vanadium",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VanadiumAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Visc",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ViscTemp",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Water",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaterAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterKarlFischer",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Zn",
                table: "AnalysisResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZnAss",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountHistoryWithVessel",
                columns: table => new
                {
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortBunkered = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateBunkered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SampleNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_VesselUsers_UserId",
                table: "VesselUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults",
                column: "VesselDetailsId",
                principalTable: "VesselDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VesselUsers_AspNetUsers_UserId",
                table: "VesselUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults");

            migrationBuilder.DropForeignKey(
                name: "FK_VesselUsers_AspNetUsers_UserId",
                table: "VesselUsers");

            migrationBuilder.DropTable(
                name: "AccountHistoryWithVessel");

            migrationBuilder.DropIndex(
                name: "IX_VesselUsers_UserId",
                table: "VesselUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VesselUsers");

            migrationBuilder.DropColumn(
                name: "AASS",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "APIGravity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ASSESS",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "AirwayNumber",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "AlSi",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "AlSiAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Aluminum",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Ant",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Appearance",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "AppearanceAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Ash",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "AshAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Asphaltenes",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "BSW",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Be",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "BunkerReceipt",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "BunkerTanker",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "BurningTemp",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "C",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CCAI",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CCAIAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Ca",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CaAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Cd",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Cetane",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CetaneAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ClientRefNumber",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CloudPoint",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CloudPointAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Co",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ColdFilterPluggingPoint",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment1",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment2",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment3",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment4",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment5",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Comment6",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Compatibility",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Confirmation",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CopperCorrosion",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Cr",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAr40Ass",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAt100",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAt40",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAt50",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAt50Ass",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "CstAt80",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Cu",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Demulsification",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Density",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "DensityAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "DieselRangeOrganics",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "DistResLoss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "EP",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FTIR",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FTIRAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Fe",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FlashPoint",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FlashPointAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FreeGlycerin",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FuelMixWithFuelOnboard",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "FuelTypeID",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "GrossHeatComb",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "H",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "H2S",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "H2SAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Halogens",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Hg",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "IBP",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Initials",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp10cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp12cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp13cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp14cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp15cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp20cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InjTemp25cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "K",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "LID",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Laboratory",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Li",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Lubricity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "LubricityAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "MCR",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "MCR10",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "MCRAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "MPT100cst",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Mg",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "MineralAcidity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "N",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "NaK",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "NaVRatio",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "NetCalVal",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "NetHeatComb",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "NetSpecificEnergy",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Neutrality",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Ni",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OffSpec1",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OffSpec2",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OffSpec3",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OffSpec4",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OxidationStability",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "OxidationStabilityAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "P",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "PAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ParticulateAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ParticulateContamination",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pb",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct10",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct20",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct30",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct40",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct50",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Pct90",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Polymer",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "PourPointAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "PourPointSummStd",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "PourPointWint",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "QuantityDiff",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SSQuantity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SampleLocation",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SampleMethod",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SampleReportDate",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SampleSentFrom",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SampleSentOn",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Se",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SealNumber",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SealNumberMARPOL",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SealNumberRetained",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SealNumberSupplier",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Sediment",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ShippingCharges",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Silicon",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Sodium",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SodiumAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Stability",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "StrongAcid",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Sulphur",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SulphurAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SulphurPPM",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SulphurPPMAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SupplierDensity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SupplierSulfur",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SupplierSulfurPPM",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SupplierSulphur",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "SupplierViscosity",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TSA",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TSE",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TSEAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TSP",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TankNumber",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Technician",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TestType",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TotalAcid",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TotalAcidAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TotalDeposits",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "TotalGlycerin",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ULO",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ULOAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Vanadium",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "VanadiumAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Visc",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ViscTemp",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Water",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "WaterAss",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "WaterKarlFischer",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Zn",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ZnAss",
                table: "AnalysisResults");

            migrationBuilder.AddColumn<string>(
                name: "IMONumber",
                table: "VesselUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "VesselUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "VesselName",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TlxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Registry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purifier",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Propulsion",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IMONumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GeneratorType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FuelSystem",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Filter",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FaxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FaxCountry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FaxArea",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExVesselName",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dwt",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Draft",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Diesel",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommissionType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Charterer",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CallSign",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ByTelex",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ByFax",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ByEMail",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Built",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillTo",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VesselDetailsId",
                table: "AnalysisResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specification",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SampleNumber",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PortBunkered",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateReceived",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateBunkered",
                table: "AnalysisResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VesselName",
                table: "AnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_VesselDetails_VesselDetailsId",
                table: "AnalysisResults",
                column: "VesselDetailsId",
                principalTable: "VesselDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
