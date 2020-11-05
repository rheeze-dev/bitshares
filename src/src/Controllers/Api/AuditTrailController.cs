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
    [Route("api/AuditTrail")]
    //[Authorize]
    public class AuditTrailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotnetdesk _dotnetdesk;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;



        public AuditTrailController(ApplicationDbContext context,
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

        // GET: api/AuditTrail
        [HttpGet("GetDeletedDatas")]
        public IActionResult GetDeletedDatas([FromRoute]Guid organizationId)
        {
            var deleted = _context.DeletedDatas.OrderByDescending(x => x.Id).ToList();
            return Json(new { data = deleted });
        }

        // GET: api/AuditTrail
        [HttpGet("GetEditedDatas")]
        public IActionResult GetEditedDatas([FromRoute]Guid organizationId)
        {
            var edited = _context.EditedDatas.OrderByDescending(x => x.Id).ToList();
            return Json(new { data = edited });
        }

    }
}