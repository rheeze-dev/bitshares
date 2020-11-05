using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace src.Migrations
{
    public partial class AuditTrail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentLedger");

            migrationBuilder.DropTable(
                name: "EmployersDeduction");

            migrationBuilder.DropTable(
                name: "SalaryLedger");

            migrationBuilder.CreateTable(
                name: "DeletedDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(nullable: true),
                    ControlNumber = table.Column<int>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EditedDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ControlNumber = table.Column<int>(nullable: false),
                    DateEdited = table.Column<DateTime>(nullable: false),
                    EditedBy = table.Column<string>(nullable: true),
                    EditedData = table.Column<string>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditedDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedDatas");

            migrationBuilder.DropTable(
                name: "EditedDatas");

            migrationBuilder.CreateTable(
                name: "CurrentLedger",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddAdjustment = table.Column<int>(nullable: false),
                    AmountOT = table.Column<double>(nullable: false),
                    AmountRH = table.Column<double>(nullable: false),
                    AmountSH = table.Column<double>(nullable: false),
                    AmountSundays = table.Column<double>(nullable: false),
                    AmountTardiness = table.Column<double>(nullable: false),
                    BasicPay = table.Column<int>(nullable: false),
                    CashOut = table.Column<int>(nullable: false),
                    Charges1 = table.Column<int>(nullable: false),
                    Charges2 = table.Column<int>(nullable: false),
                    Cola = table.Column<int>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    DateAndTime = table.Column<DateTime>(nullable: true),
                    DaysOfWorkBP = table.Column<int>(nullable: false),
                    Editor = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: true),
                    GrossPay = table.Column<double>(nullable: false),
                    GrossPayPayslip = table.Column<double>(nullable: false),
                    IdNumber = table.Column<string>(nullable: true),
                    LessAdjustment = table.Column<int>(nullable: false),
                    LoanAmount = table.Column<double>(nullable: false),
                    LoanBalance = table.Column<double>(nullable: false),
                    MidMonth = table.Column<bool>(nullable: false),
                    NetAmountPaid = table.Column<double>(nullable: false),
                    NumberOfDaysRH = table.Column<int>(nullable: false),
                    NumberOfHrsSH = table.Column<int>(nullable: false),
                    NumberOfMinOT = table.Column<int>(nullable: false),
                    NumberOfMinSundays = table.Column<int>(nullable: false),
                    NumberOfMinTardiness = table.Column<int>(nullable: false),
                    PagibigEmployee = table.Column<double>(nullable: false),
                    PagibigEmployer = table.Column<double>(nullable: false),
                    PaymentPlan = table.Column<int>(nullable: true),
                    PhilHealthEmployee = table.Column<double>(nullable: false),
                    PhilHealthEmployer = table.Column<double>(nullable: false),
                    SSSEmployee = table.Column<double>(nullable: false),
                    SSSEmployer = table.Column<double>(nullable: false),
                    SalaryLoan = table.Column<int>(nullable: false),
                    SalaryLoanChecker = table.Column<bool>(nullable: false),
                    TotalAmountBP = table.Column<double>(nullable: false),
                    TotalBasicPay = table.Column<int>(nullable: false),
                    TotalDeductions = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployersDeduction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PagibigTotal = table.Column<double>(nullable: false),
                    PhilhealthTotal = table.Column<double>(nullable: false),
                    SssTotal = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployersDeduction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryLedger",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddAdjustment = table.Column<int>(nullable: false),
                    AmountOT = table.Column<double>(nullable: false),
                    AmountRH = table.Column<double>(nullable: false),
                    AmountSH = table.Column<double>(nullable: false),
                    AmountSundays = table.Column<double>(nullable: false),
                    AmountTardiness = table.Column<double>(nullable: false),
                    BasicPay = table.Column<int>(nullable: false),
                    CashOut = table.Column<int>(nullable: false),
                    Charges1 = table.Column<int>(nullable: false),
                    Charges2 = table.Column<int>(nullable: false),
                    Cola = table.Column<int>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    DateAndTime = table.Column<DateTime>(nullable: true),
                    DaysOfWorkBP = table.Column<int>(nullable: false),
                    Editor = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: true),
                    GrossPay = table.Column<double>(nullable: false),
                    GrossPayPayslip = table.Column<double>(nullable: false),
                    IdNumber = table.Column<string>(nullable: true),
                    LessAdjustment = table.Column<int>(nullable: false),
                    LoanAmount = table.Column<double>(nullable: false),
                    LoanBalance = table.Column<double>(nullable: false),
                    MidMonth = table.Column<bool>(nullable: false),
                    NetAmountPaid = table.Column<double>(nullable: false),
                    NumberOfDaysRH = table.Column<int>(nullable: false),
                    NumberOfHrsSH = table.Column<int>(nullable: false),
                    NumberOfMinOT = table.Column<int>(nullable: false),
                    NumberOfMinSundays = table.Column<int>(nullable: false),
                    NumberOfMinTardiness = table.Column<int>(nullable: false),
                    PagibigEmployee = table.Column<double>(nullable: false),
                    PagibigEmployer = table.Column<double>(nullable: false),
                    PaymentPlan = table.Column<int>(nullable: true),
                    PhilHealthEmployee = table.Column<double>(nullable: false),
                    PhilHealthEmployer = table.Column<double>(nullable: false),
                    SSSEmployee = table.Column<double>(nullable: false),
                    SSSEmployer = table.Column<double>(nullable: false),
                    SalaryLoan = table.Column<int>(nullable: false),
                    SalaryLoanChecker = table.Column<bool>(nullable: false),
                    TotalAmountBP = table.Column<double>(nullable: false),
                    TotalBasicPay = table.Column<int>(nullable: false),
                    TotalDeductions = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryLedger", x => x.Id);
                });
        }
    }
}
