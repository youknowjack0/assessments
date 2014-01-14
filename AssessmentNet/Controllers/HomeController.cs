using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AssessmentNet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentNet.Controllers
{
    [Authorize(Roles = "admin, testee")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> _userManager;


        public HomeController()
        {
            db = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index()
        {
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());

            List<TestRun> tests = db.TestRun.Where(x => x.Testee.Id == user.Id && x.Expires > DateTime.UtcNow).ToList();
            return View(tests);
        }

        public ActionResult PreTest(string s)
        {
            throw new NotImplementedException();
        }
    }
}