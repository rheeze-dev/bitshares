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
    [Route("api/TimeClock")]
    //[Authorize]
    public class TimeClockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotnetdesk _dotnetdesk;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;



        public TimeClockController(ApplicationDbContext context,
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

        // GET: api/TimeClock/GetTimeClock
        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetTimeClockAsync([FromRoute]Guid organizationId)
        {
            var info = await _userManager.GetUserAsync(User);

            var employees = _context.Employees.ToList();
            var employee = _context.Employees.Where(x => x.IdNumber == info.IdNumber);

            if (info.Role == "Employee")
            {
                return Json(new { data = employee });
            }
            else if (info.Role == "Manager")
            {
                return Json(new { data = employees });
            }
            else
            {
                return Json(new { data = employees });
            }
        }

        //POST: api/TimeClock/PostTimeIn
        [HttpPost("PostTimeIn")]
        public async Task<IActionResult> PostTimeIn(string idNumber)
        {
            Employees employees = _context.Employees.Where(x => x.IdNumber == idNumber).FirstOrDefault();
            Attendance currentAttendance = _context.Attendance.OrderByDescending(x => x.TimeInAM).Where(x => x.IdNumber == idNumber).FirstOrDefault();
            //MonthlyTimeSheet monthlyTimeSheet = _context.MonthlyTimeSheet.Where(x => x.IdNumber == idNumber).OrderByDescending(x => x.Date).First();

            var info = await _userManager.GetUserAsync(User);
            var totalTimeIn = employees.TotalTimeIn;
            employees.Email = employees.Email;
            employees.TotalTimeIn = totalTimeIn + 1;
            var globalTardiness = 0;
            if (currentAttendance == null || currentAttendance.TimeInAM.Value.Date != DateTime.Now.Date)
            {
                Attendance attendance = new Attendance
                {
                    IdNumber = idNumber,
                    FullName = employees.FullName
                    //TimeInAM = DateTime.Now,
                    //EditorTimeIn = info.FullName,
                };
                var getLastControlNumber = _context.Attendance.OrderByDescending(x => x.ControlNumber).Select(x => x.ControlNumber).FirstOrDefault();
                attendance.ControlNumber = getLastControlNumber + 1;

                var afternoon = DateTime.Now;
                afternoon = new DateTime(afternoon.Year, afternoon.Month, afternoon.Day, 12, 50, 00);
                if (DateTime.Now >= afternoon)
                {
                    attendance.TimeInPM = DateTime.Now;

                    var tardiness = DateTime.Now;
                    tardiness = new DateTime(tardiness.Year, tardiness.Month, tardiness.Day, 13, 00, 00);
                    TimeSpan solve = attendance.TimeInPM.Value - tardiness;
                    int tardinessMin = (int)solve.TotalMinutes;

                    if (attendance.TimeInPM.Value > tardiness)
                    {
                        attendance.NumberOfMinTardinessPM = tardinessMin;
                        globalTardiness = tardinessMin;
                    }
                }
                else
                {
                    attendance.TimeInAM = DateTime.Now;

                    var tardiness = DateTime.Now;
                    tardiness = new DateTime(tardiness.Year, tardiness.Month, tardiness.Day, 08, 00, 00);
                    TimeSpan solve = attendance.TimeInAM.Value - tardiness;
                    int tardinessMin = (int)solve.TotalMinutes;

                    if (attendance.TimeInAM.Value > tardiness)
                    {
                        attendance.NumberOfMinTardinessAM = tardinessMin;
                    }
                }

                //var tardiness = DateTime.Now;
                //tardiness = new DateTime(tardiness.Year, tardiness.Month, tardiness.Day, 08, 00, 00);
                //TimeSpan solve = attendance.TimeInAM.Value - tardiness;
                //int tardinessMin = (int)solve.TotalMinutes;

                //if (attendance.TimeInAM.Value > tardiness)
                //{
                //    attendance.NumberOfMinTardiness = tardinessMin;
                //}

                attendance.Id = Guid.NewGuid();
                attendance.TotalNumberOfMinTardiness = attendance.NumberOfMinTardinessAM + attendance.NumberOfMinTardinessPM;
                _context.Attendance.Add(attendance);

                //_context.MonthlyTimeSheet.Add(attendance);
            }
            else
            {
                if (currentAttendance.TimeInPM != null)
                {
                    return Json(new { success = false, message = "Your Clock Punch for today is already completed!" });
                }
                currentAttendance.TimeInPM = DateTime.Now.AddHours(5);

                var tardiness = DateTime.Now;
                tardiness = new DateTime(tardiness.Year, tardiness.Month, tardiness.Day, 13, 00, 00);
                TimeSpan solve = currentAttendance.TimeInPM.Value - tardiness;
                int tardinessMin = (int)solve.TotalMinutes;

                if (currentAttendance.TimeInPM.Value > tardiness)
                {
                    currentAttendance.NumberOfMinTardinessPM = tardinessMin;
                }

                //currentAttendance.Id = Guid.NewGuid();
                currentAttendance.TotalNumberOfMinTardiness = currentAttendance.NumberOfMinTardinessAM + currentAttendance.NumberOfMinTardinessPM;
                _context.Attendance.Update(currentAttendance);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Time in successful!" });
        }

        //POST: api/TimeClock/PostTimeOut
        [HttpPost("PostTimeOut")]
        public async Task<IActionResult> PostTimeOut(string idNumber)
        {
            Employees employees = _context.Employees.Where(x => x.IdNumber == idNumber).FirstOrDefault();
            Attendance attendance = _context.Attendance.OrderByDescending(x => x.TimeInAM).Where(x => x.IdNumber == idNumber && x.TimeOutPM == null).FirstOrDefault();
            MonthlyTimeSheet monthlyTimeSheet = _context.MonthlyTimeSheet.Where(x => x.IdNumber == idNumber).OrderByDescending(x => x.Date).FirstOrDefault();

            var info = await _userManager.GetUserAsync(User);
            var totalTimeOut = employees.TotalTimeOut;
            employees.TotalTimeOut = totalTimeOut + 1;
            //int finalWorkTime = 0;
            int numberOfMinWorkedAM = 0;
            int numberOfMinWorkedPM = 0;

            //attendance.TimeOutAM = DateTime.Now;
            var afternoon = DateTime.Now;
            afternoon = new DateTime(afternoon.Year, afternoon.Month, afternoon.Day, 16, 00, 00);
            //if (afternoon.AddHours(1) >= afternoon)
            if (DateTime.Now >= afternoon)
                {
                attendance.TimeOutPM = DateTime.Now;

                TimeSpan diff = attendance.TimeOutPM.Value - attendance.TimeInPM.Value;
                numberOfMinWorkedPM = (int)diff.TotalMinutes;
                //int finalMinWorked = numberOfMinWorked - 60;
                //finalWorkTime = finalMinWorked;

                //var adjustment = Math.Abs(numberOfMinWorked - 540);
            }
            else
            {
                attendance.TimeOutAM = DateTime.Now;

                TimeSpan diff = attendance.TimeOutAM.Value - attendance.TimeInAM.Value;
                numberOfMinWorkedAM = (int)diff.TotalMinutes;
                //int finalMinWorked = numberOfMinWorked - 60;
                //finalWorkTime = finalMinWorked;

                //var adjustment = Math.Abs(numberOfMinWorked - 540);
            }
            //attendance.EditorTimeOut = info.FullName;

            //TimeSpan diff = attendance.TimeOutAM.Value - attendance.TimeInAM.Value;
            //int numberOfMinWorked = (int)diff.TotalMinutes;
            //int finalMinWorked = numberOfMinWorked - 60;

            //var adjustment = Math.Abs(numberOfMinWorked - 540);

            //DateTime start = DateTime.Now;
            //start = new DateTime(start.Year, start.Month, start.Day, 08, 00, 00);

            //DateTime end = DateTime.Now;
            //end = new DateTime(end.Year, end.Month, end.Day, 17, 00, 00);

            //attendance.NumberOfMinWorked = numberOfMinWorkedAM + numberOfMinWorkedPM;

            //if (attendance.TimeInAM.Value.DayOfWeek != DayOfWeek.Sunday)
            //{
            //}

            //else if (attendance.TimeInAM.Value.DayOfWeek == DayOfWeek.Sunday)
            //{
            //    if (attendance.TimeInAM.Value < start)
            //    {
            //        DateTime timeIn = start;
            //        if (attendance.TimeOutAM.Value > end)
            //        {
            //            DateTime timeOut = end;
            //            TimeSpan difference = timeOut - timeIn;
            //            int numberOfMinSundays = (int)difference.TotalMinutes;
            //            attendance.NumberOfMinSunday = numberOfMinSundays;
            //        }
            //        else if (attendance.TimeOutAM.Value < end)
            //        {
            //            DateTime timeOut = attendance.TimeOutAM.Value;
            //            TimeSpan difference = timeOut - timeIn;
            //            int numberOfMinSundays = (int)difference.TotalMinutes;
            //            attendance.NumberOfMinSunday = numberOfMinSundays;
            //        }
            //    }
            //    else if (attendance.TimeInAM.Value > start)
            //    {
            //        DateTime timeIn = attendance.TimeInAM.Value;
            //        if (attendance.TimeOutAM.Value > end)
            //        {
            //            DateTime timeOut = end;
            //            TimeSpan difference = timeOut - timeIn;
            //            int numberOfMinSundays = (int)difference.TotalMinutes;
            //            attendance.NumberOfMinSunday = numberOfMinSundays;
            //        }
            //        else if (attendance.TimeOutAM.Value < end)
            //        {
            //            DateTime timeOut = attendance.TimeOutAM.Value;
            //            TimeSpan difference = timeOut - timeIn;
            //            int numberOfMinSundays = (int)difference.TotalMinutes;
            //            attendance.NumberOfMinSunday = numberOfMinSundays;
            //        }
            //    }
            //}

            //attendance.NumberOfMinOT = 0;

            attendance.NumberOfMinWorked = numberOfMinWorkedAM + numberOfMinWorkedPM;
            monthlyTimeSheet.TotalNumberOfMinWorked = monthlyTimeSheet.TotalNumberOfMinWorked + attendance.NumberOfMinWorked;
            _context.Attendance.Update(attendance);
            _context.MonthlyTimeSheet.Update(monthlyTimeSheet);

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Time out successful!" });
        }

        //POST: api/TimeClock/PostOvertime
        [HttpPost("PostOvertime")]
        public async Task<IActionResult> PostOvertime(string idNumber)
        {
            Employees employees = _context.Employees.Where(x => x.IdNumber == idNumber).FirstOrDefault();
            Attendance attendance = _context.Attendance.Where(x => x.IdNumber == idNumber && x.TimeOutAM == null).FirstOrDefault();
            var info = await _userManager.GetUserAsync(User);
            var totalTimeOut = employees.TotalTimeOut;
            employees.TotalTimeOut = totalTimeOut + 1;

            //var checker = employees.DateTimeChecker.Value.AddHours(1);
            //if (checker > DateTime.Now)
            //{
            //    return Json(new { success = false, message = "Minumum time is 1 hour!" });
            //}
            //else
            //{
            //    employees.DateTimeChecker = DateTime.Now;
            //    _context.Employees.Update(employees);
            //}

            attendance.TimeOutAM = DateTime.Now;
            //attendance.EditorTimeOut = info.FullName;

            TimeSpan diff = attendance.TimeOutAM.Value - attendance.TimeInAM.Value;
            int numberOfMinWorked = (int)diff.TotalMinutes;
            int finalMinWorked = numberOfMinWorked - 60;

            var adjustment = Math.Abs(numberOfMinWorked - 540);

            DateTime start = DateTime.Now;
            start = new DateTime(start.Year, start.Month, start.Day, 08, 00, 00);

            DateTime end = DateTime.Now;
            end = new DateTime(end.Year, end.Month, end.Day, 17, 00, 00);

            if (attendance.TimeInAM.Value.DayOfWeek != DayOfWeek.Sunday)
            {
                attendance.NumberOfMinWorked = finalMinWorked;
            }

            else if (attendance.TimeInAM.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                if (attendance.TimeInAM.Value < start)
                {
                    DateTime timeIn = start;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
                else if (attendance.TimeInAM.Value > start)
                {
                    DateTime timeIn = attendance.TimeInAM.Value;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
            }

            var overtime = DateTime.Now;
            overtime = new DateTime(overtime.Year, overtime.Month, overtime.Day, 17, 00, 00);
            TimeSpan solve = attendance.TimeOutAM.Value - overtime;
            int overtimeMin = (int)solve.TotalMinutes;

            if (attendance.TimeOutAM.Value > overtime)
            {
                attendance.NumberOfMinOT = overtimeMin;
            }

            _context.Attendance.Update(attendance);

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Time out successful!" });
        }

        //POST: api/TimeClock/PostRegularHoliday
        [HttpPost("PostRegularHoliday")]
        public async Task<IActionResult> PostRegularHoliday(string idNumber)
        {
            Employees employees = _context.Employees.Where(x => x.IdNumber == idNumber).FirstOrDefault();
            Attendance attendance = _context.Attendance.Where(x => x.IdNumber == idNumber && x.TimeOutAM == null).FirstOrDefault();
            var info = await _userManager.GetUserAsync(User);
            var totalTimeOut = employees.TotalTimeOut;
            employees.TotalTimeOut = totalTimeOut + 1;

            //var checker = employees.DateTimeChecker.Value.AddHours(1);
            //if (checker > DateTime.Now)
            //{
            //    return Json(new { success = false, message = "Minumum time is 1 hour!" });
            //}
            //else
            //{
            //    employees.DateTimeChecker = DateTime.Now;
            //    _context.Employees.Update(employees);
            //}

            attendance.TimeOutAM = DateTime.Now;
            //attendance.EditorTimeOut = info.FullName;

            TimeSpan diff = attendance.TimeOutAM.Value - attendance.TimeInAM.Value;
            int numberOfMinWorked = (int)diff.TotalMinutes;
            int finalMinWorked = numberOfMinWorked - 60;

            var adjustment = Math.Abs(numberOfMinWorked - 540);

            DateTime start = DateTime.Now;
            start = new DateTime(start.Year, start.Month, start.Day, 08, 00, 00);

            DateTime end = DateTime.Now;
            end = new DateTime(end.Year, end.Month, end.Day, 17, 00, 00);

            if (attendance.TimeInAM.Value.DayOfWeek != DayOfWeek.Sunday)
            {
                attendance.NumberOfMinWorked = finalMinWorked;
            }

            else if (attendance.TimeInAM.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                if (attendance.TimeInAM.Value < start)
                {
                    DateTime timeIn = start;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
                else if (attendance.TimeInAM.Value > start)
                {
                    DateTime timeIn = attendance.TimeInAM.Value;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
            }

            attendance.NumberOfMinOT = 0;

            _context.Attendance.Update(attendance);

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Time out successful!" });
        }

        //POST: api/TimeClock/PostSpecialHoliday
        [HttpPost("PostSpecialHoliday")]
        public async Task<IActionResult> PostSpecialHoliday(string idNumber)
        {
            Employees employees = _context.Employees.Where(x => x.IdNumber == idNumber).FirstOrDefault();
            Attendance attendance = _context.Attendance.Where(x => x.IdNumber == idNumber && x.TimeOutAM == null).FirstOrDefault();
            var info = await _userManager.GetUserAsync(User);
            var totalTimeOut = employees.TotalTimeOut;
            employees.TotalTimeOut = totalTimeOut + 1;

            //var checker = employees.DateTimeChecker.Value.AddHours(1);
            //if (checker > DateTime.Now)
            //{
            //    return Json(new { success = false, message = "Minumum time is 1 hour!" });
            //}
            //else
            //{
            //    employees.DateTimeChecker = DateTime.Now;
            //    _context.Employees.Update(employees);
            //}

            attendance.TimeOutAM = DateTime.Now;
            //attendance.EditorTimeOut = info.FullName;

            TimeSpan diff = attendance.TimeOutAM.Value - attendance.TimeInAM.Value;
            int numberOfMinWorked = (int)diff.TotalMinutes;
            int finalMinWorked = numberOfMinWorked - 60;

            var adjustment = Math.Abs(numberOfMinWorked - 540);

            DateTime start = DateTime.Now;
            start = new DateTime(start.Year, start.Month, start.Day, 08, 00, 00);

            DateTime end = DateTime.Now;
            end = new DateTime(end.Year, end.Month, end.Day, 17, 00, 00);

            if (attendance.TimeInAM.Value.DayOfWeek != DayOfWeek.Sunday)
            {
                attendance.NumberOfMinWorked = finalMinWorked;
            }

            else if (attendance.TimeInAM.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                if (attendance.TimeInAM.Value < start)
                {
                    DateTime timeIn = start;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
                else if (attendance.TimeInAM.Value > start)
                {
                    DateTime timeIn = attendance.TimeInAM.Value;
                    if (attendance.TimeOutAM.Value > end)
                    {
                        DateTime timeOut = end;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                    else if (attendance.TimeOutAM.Value < end)
                    {
                        DateTime timeOut = attendance.TimeOutAM.Value;
                        TimeSpan difference = timeOut - timeIn;
                        int numberOfMinSundays = (int)difference.TotalMinutes;
                        attendance.NumberOfMinSunday = numberOfMinSundays;
                    }
                }
            }

            attendance.NumberOfMinOT = 0;

            _context.Attendance.Update(attendance);

            var end2 = DateTime.Now;
            end2 = new DateTime(end2.Year, end2.Month, end2.Day, 17, 00, 00);
          
            var start2 = DateTime.Now;
            start2 = new DateTime(start2.Year, start2.Month, start2.Day, 08, 00, 00);
            DateTime timeIn2;
            if (attendance.TimeInAM.Value < start2)
            {
                timeIn2 = start2;
                if (attendance.TimeOutAM.Value > end2)
                {
                    DateTime timeOut2 = end2;
                    TimeSpan solve2 = timeOut2 - timeIn2;
                    int numberOfHrsSH = (int)solve2.TotalHours;
                }
                else if (attendance.TimeOutAM.Value < end2)
                {
                    DateTime timeOut2 = attendance.TimeOutAM.Value;
                    TimeSpan solve2 = timeOut2 - timeIn2;
                    int numberOfHrsSH = (int)solve2.TotalHours;
                }
            }
            else if (attendance.TimeInAM.Value > start2)
            {
                timeIn2 = attendance.TimeInAM.Value;
                if (attendance.TimeOutAM.Value > end2)
                {
                    DateTime timeOut2 = end2;
                    TimeSpan solve2 = timeOut2 - timeIn2;
                    int numberOfHrsSH = (int)solve2.TotalHours;
                }
                else if (attendance.TimeOutAM.Value < end2)
                {
                    DateTime timeOut2 = attendance.TimeOutAM.Value;
                    TimeSpan solve2 = timeOut2 - timeIn2;
                    int numberOfHrsSH = (int)solve2.TotalHours;
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Time out successful!" });
        }

    }
}