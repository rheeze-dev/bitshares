using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using src.Data;
using src.Models;
using src.Services;

namespace src.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/MonthlyTimeSheet")]
    //[Authorize]
    public class MonthlyTimeSheetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotnetdesk _dotnetdesk;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;



        public MonthlyTimeSheetController(ApplicationDbContext context,
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

        // GET: api/Attendance/GetAttendance
        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetMonthlyTimeSheetAsync([FromRoute]Guid organizationId)
        {
            var info = await _userManager.GetUserAsync(User);

            var allTimeSheets = _context.MonthlyTimeSheet.ToList();
            var myTimeSheet = _context.MonthlyTimeSheet.Where(x => x.IdNumber == info.IdNumber);

            if (info.Role == "Employee")
            {
                return Json(new { data = myTimeSheet });
            }
            else if (info.Role == "Manager")
            {
                return Json(new { data = allTimeSheets });
            }
            else
            {
                return Json(new { data = allTimeSheets });
            }
        }

        // GET: api/Deductions/PostDeductions
        [HttpPost]
        public async Task<IActionResult> PostAttendance([FromBody] JObject model)
        {
            Guid objGuid = Guid.Empty;
            objGuid = Guid.Parse(model["Id"].ToString());
            var info = await _userManager.GetUserAsync(User);

            //int id = 0;
            //id = Convert.ToInt32(model["Id"].ToString());
            Attendance attendance = _context.Attendance.Where(x => x.Id == objGuid).FirstOrDefault();
            var originalTardiness = attendance.TotalNumberOfMinTardiness;
            var originalTimeIn = attendance.TimeInAM;
            var threeDaysAgo = DateTime.Now.AddDays(-3);
            if (originalTimeIn < threeDaysAgo)
            {
                return Json(new { success = false, message = "Maximum time to edit is after 3 days!" });
            }

            //var currentAttendance = _context.Attendance.Where(x => x.Id == objGuid).FirstOrDefault();
            if (attendance.TimeInAM != Convert.ToDateTime(model["TimeInAM"].ToString()) || attendance.TimeInPM != Convert.ToDateTime(model["TimeInPM"].ToString()) || attendance.TimeOutAM != Convert.ToDateTime(model["TimeOutAM"].ToString()) || attendance.TimeOutPM != Convert.ToDateTime(model["TimeOutPM"].ToString()) || attendance.Remarks != model["Remarks"].ToString())
            {
                var one = "";
                var two = "";
                var three = "";
                var four = "";
                var five = "";
                EditedDatas editedDatas = new EditedDatas
                {
                    DateEdited = DateTime.Now,
                    Origin = "Attendance",
                    EditedBy = info.FullName,
                    ControlNumber = attendance.ControlNumber
                };
                if (attendance.TimeInAM != Convert.ToDateTime(model["TimeInAM"].ToString()))
                {
                    one = "Time in AM = " + attendance.TimeInAM + " - " + Convert.ToDateTime(model["TimeInAM"].ToString()) + "; ";
                }
                if (attendance.TimeOutAM != Convert.ToDateTime(model["TimeOutAM"].ToString()))
                {
                    two = " Time out AM = " + attendance.TimeOutAM + " - " + Convert.ToDateTime(model["TimeOutAM"].ToString()) + "; ";
                }
                if (attendance.TimeInPM != Convert.ToDateTime(model["TimeInPM"].ToString()))
                {
                    three = " Time in PM = " + attendance.TimeInPM + " - " + Convert.ToDateTime(model["TimeInPM"].ToString()) + "; ";
                }
                if (attendance.TimeOutPM != Convert.ToDateTime(model["TimeOutPM"].ToString()))
                {
                    four = " Time out PM = " + attendance.TimeOutPM + " - " + Convert.ToDateTime(model["TimeOutPM"].ToString()) + "; ";
                }
                if (attendance.Remarks != model["Remarks"].ToString())
                {
                    five = " Remarks = " + attendance.Remarks + " - " + model["Remarks"].ToString() + "; ";
                }
                var datas = one + two + three + four + five;
                editedDatas.EditedData = datas;
                _context.EditedDatas.Add(editedDatas);
            }

            attendance.TimeInAM = Convert.ToDateTime(model["TimeInAM"].ToString());
            attendance.TimeInPM = Convert.ToDateTime(model["TimeInPM"].ToString());
            attendance.TimeOutAM = Convert.ToDateTime(model["TimeOutAM"].ToString());
            attendance.TimeOutPM = Convert.ToDateTime(model["TimeOutPM"].ToString());
            attendance.Remarks = model["Remarks"].ToString();
            if (model["Remarks"].ToString() == "")
            {
                return Json(new { success = false, message = "Remarks cannot be empty!" });
            }

            var newTimeIn = Convert.ToDateTime(model["TimeInAM"].ToString());
            var newTimeInDate = newTimeIn.Date;
            if (originalTimeIn.Value.Date != newTimeInDate)
            {
                return Json(new { success = false, message = "You can only edit the time!" });
            }
            attendance.Editor = info.FullName;

            var tardiness = DateTime.Now;
            tardiness = new DateTime(tardiness.Year, tardiness.Month, tardiness.Day, 08, 00, 00);
            TimeSpan solve = attendance.TimeInAM.Value - tardiness;
            int tardinessMin = (int)solve.TotalMinutes;

            Employees employees = _context.Employees.Where(x => x.IdNumber == model["IdNumber"].ToString()).FirstOrDefault();

            if (attendance.TimeInAM.Value > tardiness)
            {
                attendance.TotalNumberOfMinTardiness = tardinessMin;
            }
            else if (attendance.TimeInAM.Value < tardiness)
            {
                attendance.TotalNumberOfMinTardiness = attendance.TotalNumberOfMinTardiness - originalTardiness;
            }

            _context.Attendance.Update(attendance);

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Successfully Saved!" });
        }

        //DELETE: api/Attendance/DeleteAttendance
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance([FromRoute] string id)
        {
            Attendance attendance = _context.Attendance.Where(x => x.IdNumber == id).FirstOrDefault();
            _context.Remove(attendance);
            var info = await _userManager.GetUserAsync(User);

            DeletedDatas deleted = new DeletedDatas
            {
                DateDeleted = DateTime.Now,
                IdNumber = attendance.IdNumber,
                Origin = "Attendance",
                FullName = attendance.FullName,
                DeletedBy = info.FullName,
                ControlNumber = attendance.ControlNumber
            };
            _context.DeletedDatas.Add(deleted);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }

    }
}