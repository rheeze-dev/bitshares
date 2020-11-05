using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using src.Data;
using src.Models;

namespace src.Controllers
{
    [Authorize]
    public class AuditTrailController : BaseDotnetDeskController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuditTrailController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(Guid org)
        {
            if (org == Guid.Empty)
            {
                org = Guid.NewGuid();
                //return NotFound();
            }
            ApplicationUser appUser = await _userManager.GetUserAsync(User);

            Organization organization = _context.Organization.Where(x => x.organizationId.Equals(org)).FirstOrDefault();
            ViewData["org"] = org;
            return View(organization);
        }

        public async Task<IActionResult> DeletedDatas(Guid org)
        {
            if (org == Guid.Empty)
            {
                org = Guid.NewGuid();
                //return NotFound();
            }
            ApplicationUser appUser = await _userManager.GetUserAsync(User);

            Organization organization = _context.Organization.Where(x => x.organizationId.Equals(org)).FirstOrDefault();
            ViewData["org"] = org;
            return View(organization);
        }

        //public IActionResult AddEditIndex(Guid org, Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        Subject subject = new Subject();
        //        //ticketing.ticketingId = org;
        //        return View(subject);
        //    }
        //    else
        //    {
        //        return View(_context.Subject.Where(x => x.Id.Equals(id)).FirstOrDefault());
        //    }

        //}



    }
}