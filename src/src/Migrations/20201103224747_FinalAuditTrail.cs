using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace src.Migrations
{
    public partial class FinalAuditTrail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DeletedDatas");

            migrationBuilder.AddColumn<string>(
                name: "IdNumber",
                table: "DeletedDatas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "DeletedDatas");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "DeletedDatas",
                nullable: true);
        }
    }
}
