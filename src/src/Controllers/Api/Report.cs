using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OfficeOpenXml;
using src.Data;
using src.Models;
using src.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace src.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Report")]
    //[Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotnetdesk _dotnetdesk;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;



        public ReportController(ApplicationDbContext context,
            IDotnetdesk dotnetdesk,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _dotnetdesk = dotnetdesk;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        [HttpGet("PayslipReport")]
        public async Task<IActionResult> PayslipReport(int Year, int Month)
        {
            // query data from database  
            await Task.Yield();

            var payslip = _context.Attendance.Where(x => x.CreateAt.Year.Equals(Year) && x.CreateAt.Month.Equals(Month)).Select(y => new { CreatedAt = y.CreateAt, Fullname = y.FullName, TimeInAM = y.TimeInAM, TimeOutAM = y.TimeOutAM, TimeInPM = y.TimeInPM, TimeOutPM = y.TimeOutPM, CreatedBy = y.CreateBy, Editor = y.Editor }).ToList();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(payslip, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Salary ledger and Payslip {DateTime.Now.ToString("MMMM-dd-yyyy")}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpGet("PayslipReportDate")]
        public async Task<IActionResult> PayslipReportDate(int Date)
        {
            // query data from database  
            await Task.Yield();

            var stream = new MemoryStream();

            if (Date == 1000000)
            {
                {
                    var all = _context.Attendance.Select(y => new {CreatedAt = y.CreateAt, Fullname = y.FullName, TimeInAM = y.TimeInAM, TimeOutAM = y.TimeOutAM, TimeInPM = y.TimeInPM, TimeOutPM = y.TimeOutPM, CreatedBy = y.CreateBy, Editor = y.Editor }).ToList();
                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        workSheet.Cells.LoadFromCollection(all, true);
                        package.Save();
                    }
                }
            }
            else if (Date == 1)
            {
                var today = _context.Attendance.Where(x => x.CreateAt >= DateTime.Today).Select(y => new { CreatedAt = y.CreateAt, Fullname = y.FullName, TimeInAM = y.TimeInAM, TimeOutAM = y.TimeOutAM, TimeInPM = y.TimeInPM, TimeOutPM = y.TimeOutPM, CreatedBy = y.CreateBy, Editor = y.Editor }).ToList();
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(today, true);
                    package.Save();
                }
            }
            else if (Date == 31)
            {
                var lastMonth = _context.Attendance.Where(x => x.CreateAt >= DateTime.Today.AddDays(-31)).Select(y => new { CreatedAt = y.CreateAt, Fullname = y.FullName, TimeInAM = y.TimeInAM, TimeOutAM = y.TimeOutAM, TimeInPM = y.TimeInPM, TimeOutPM = y.TimeOutPM, CreatedBy = y.CreateBy, Editor = y.Editor }).ToList();
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(lastMonth, true);
                    package.Save();
                }
            }
            else if (Date == 365)
            {
                var lastYear = _context.Attendance.Where(x => x.CreateAt >= DateTime.Today.AddDays(-365)).Select(y => new { CreatedAt = y.CreateAt, Fullname = y.FullName, TimeInAM = y.TimeInAM, TimeOutAM = y.TimeOutAM, TimeInPM = y.TimeInPM, TimeOutPM = y.TimeOutPM, CreatedBy = y.CreateBy, Editor = y.Editor }).ToList();
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(lastYear, true);
                    package.Save();
                }
            }

            stream.Position = 0;
            string excelName = $"Salary ledger and Payslip {DateTime.Now.ToString("MMMM-dd-yyyy")}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}