using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace src.Migrations
{
    public partial class MonthlyTimeSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlyTimeSheet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ControlNumber = table.Column<int>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    Editor = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    NumberOfMinOT = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    TotalNumberOfMinTardiness = table.Column<int>(nullable: false),
                    TotalNumberOfMinWorked = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTimeSheet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyTimeSheet");
        }
    }
}
