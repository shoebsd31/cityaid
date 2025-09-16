using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityAid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TeamType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromState = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToState = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SharePointUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TeamType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SensitivityLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_ApprovedBy",
                table: "ApprovalHistory",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_CaseId",
                table: "ApprovalHistory",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_CreatedAt",
                table: "ApprovalHistory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CityCode_TeamType",
                table: "Cases",
                columns: new[] { "CityCode", "TeamType" });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CreatedAt",
                table: "Cases",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_State",
                table: "Cases",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CaseId",
                table: "Files",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CityCode_TeamType",
                table: "Files",
                columns: new[] { "CityCode", "TeamType" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreatedAt",
                table: "Files",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalHistory");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Cases");
        }
    }
}
