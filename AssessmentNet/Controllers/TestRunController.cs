using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AssessmentNet.Models;
using AssessmentNet.ViewModels.TestRun;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentNet.Controllers
{
    [Authorize(Roles = "testee, admin")]
    public class TestRunController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> _userManager;


        public TestRunController()
        {
            db = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index(int testrun)
        {
            TestRun run = db.TestRun.FirstOrDefault(x => x.Id == testrun);

            ActionResult result;
            if (!IsCurrentRun(run, out result))
                return result;

            return View(run);
        }

        private bool IsCurrentRun(TestRun run, out ActionResult result)
        {
            if (!IsValidTestRun(run, out result))
                return false;

            if (!IsNonExpiredRun(run, out result))
                return false;

            return true;
        }

        private bool IsNonExpiredRun(TestRun run, out ActionResult result)
        {
            if (run.TimeToLive() <= new TimeSpan(0))
            {
                result= TextError("This test has expired");
                return false;
            }

            result = null;
            return true;
        }

        public ActionResult BeginTestRun(int testrun)
        {
            TestRun run = db.TestRun.FirstOrDefault(x => x.Id == testrun);

            ActionResult actionResult;

            if (!IsValidTestRun(run, out actionResult)) 
                return actionResult;

            //started already?
            if (run.HasStarted)
                return RedirectToAction("Index", new { testrun });

            //ok!
            run.HasStarted = true;
            run.Started = DateTime.UtcNow;

            db.Entry(run).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index", new {testrun});

        }

        private bool IsValidTestRun(TestRun run, out ActionResult actionResult)
        {
            //run exists?
            if (run == null)
            {
                actionResult = TextError("Test run not found");
                return false;
            }

            //correct user?
            ApplicationUser user = _userManager.FindById(User.Identity.GetUserId());
            if (run.Testee != user)
            {
                actionResult = TextError("Test run not found");
                return false;
            }

            actionResult = null;
            return true;
        }

        private ActionResult TextError(string message)
        {
            return new ContentResult(){Content = message};
        }

        public ActionResult AnswerQuestion(int testrun, int question, TestRunResponseViewModel answer)
        {
            
        }

        public ActionResult EnterQuestion(int testrun, int question)
        {
            
        }

        public ActionResult ExitQuestion(int testrun, int question)
        {
            
        }

    }
}