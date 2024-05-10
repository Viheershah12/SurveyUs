using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SurveyUs.Infrastructure.Migrations.ApplicationDb
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AuditLogs",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>("nvarchar(max)", nullable: true),
                    Type = table.Column<string>("nvarchar(max)", nullable: true),
                    TableName = table.Column<string>("nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>("datetime2", nullable: false),
                    OldValues = table.Column<string>("nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>("nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>("nvarchar(max)", nullable: true),
                    PrimaryKey = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AuditLogs", x => x.Id); });

            migrationBuilder.CreateTable(
                "Brand",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    Tax = table.Column<decimal>("decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>("nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Brand", x => x.Id); });

            migrationBuilder.CreateTable(
                "Products",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>("nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>("varbinary(max)", nullable: true),
                    Description = table.Column<string>("nvarchar(max)", nullable: true),
                    Rate = table.Column<decimal>("decimal(18,2)", nullable: false),
                    BrandId = table.Column<int>("int", nullable: false),
                    CreatedBy = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>("datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>("nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>("datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        "FK_Products_Brand_BrandId",
                        x => x.BrandId,
                        "Brand",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Products_BrandId",
                "Products",
                "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AuditLogs");

            migrationBuilder.DropTable(
                "Products");

            migrationBuilder.DropTable(
                "Brand");
        }
    }
}