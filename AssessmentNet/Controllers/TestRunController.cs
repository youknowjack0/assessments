using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AssessmentNet.Models;
using AssessmentNet.ViewModels.TestRunViewModels;
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

            if (!run.Responses.Any(x => x.CanAnswer()))
                ViewBag.IsComplete = true;
                

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnswerQuestion(TestRunResponseViewModel answer)
        {
            if (answer == null || answer.Answers == null)
                return TextError("Invalid Request");

            int testrun = answer.TestRunId;
            int question = answer.QuestionResponseId;

            TestRun run = db.TestRun.Single(x => x.Id == answer.TestRunId);

            ActionResult r;
            if (!IsCurrentRun(run, out r))
                return TextError("Invalid test run");

            QuestionResponse qr = db.Responses.Single(x => x.Id == answer.QuestionResponseId);

            if (qr.TestRun.Id != run.Id)
                return TextError("Invalid request");

            string message;

            if (qr.CanAnswer()) //not timed out
            {
                foreach (var item in answer.Answers.Where( x=>x.Selected))
                {
                    var a = db.Answers.Single(x => x.Id == item.AnswerId);
                    if (a == null)
                        return TextError("Invalid answer in request");
                    db.QuestionResponseAnswers.Add(new QuestionResponseAnswer {Answer = a, Response = qr});
                }

                qr.Finished = DateTime.UtcNow;

                db.SaveChanges();
                message =  "The previous answer(s) were submitted successfully";
            }
            else
            {
                message = "<strong>The previous answer wasn't submitted successfully, due to: Exceeded allowed time</strong>";
            }

            return EnterQuestion(testrun, message);
        }

        /// <summary>
        /// enter the next available question, with a message
        /// </summary>
        private ActionResult EnterQuestion(int testrun, string message)
        {
            ViewBag.Message = message;

            TestRun run = db.TestRun.FirstOrDefault(x => x.Id == testrun);

            ActionResult r;
            if (!IsCurrentRun(run, out r))
                return r;

            //find next question
            var next = db.Responses
                .Where(x => x.TestRun.Id == testrun && x.Finished == null)
                .ToList()
                .FirstOrDefault(x => x.CanAnswer());

            if (next == null)
                return RedirectToAction("Index", new {testrun});
            else
            {
                return RedirectToAction("EnterQuestion", new { testrun, question = next.Id});
            }

        }

        public ActionResult EnterQuestion(int testrun, int question)
        {
            TestRun run = db.TestRun.FirstOrDefault(x => x.Id == testrun);

            ActionResult r;
            if (!IsCurrentRun(run, out r))
                return r;

            QuestionResponse q = db.Responses.FirstOrDefault(x => x.Id == question);

            if (q == null)
                return TextError("Invalid request");

            Trace.Assert(q.TestRun.Testee == _userManager.FindById(User.Identity.GetUserId()));

            if (q.Finished != null)
                return RedirectToAction("Index", new {testrun});

            if(!q.CanAnswer())
                return RedirectToAction("Index", new { testrun });

            if (q.Started == null)
            {
                q.Started = DateTime.UtcNow;
                db.SaveChanges();
            }

            var answers =
                ((MultiChoiceQuestion) (q.Question)).Answers.Select(
                    x => new AnswerSelectionViewModel {AnswerHtml = x.AnswerHtml, AnswerId = x.Id, Selected = false});

            var trrvm = new TestRunResponseViewModel
            {
                QuestionResponseId = question,
                TestRunId = testrun,
                TestRun = run,
                QuestionResponse = q,
                Remaining = q.GetRemaining() - TimeSpan.FromSeconds(TestRun.GraceSeconds),
                Answers = answers
            };

            return View(trrvm);
        }

        public ActionResult PreTest(int testrun)
        {
            TestRun run = db.TestRun.FirstOrDefault(x => x.Id == testrun);

            ActionResult actionResult;

            if (!IsValidTestRun(run, out actionResult))
                return actionResult;

            //started already?
            if (run.HasStarted)
                return RedirectToAction("Index", new { testrun });

            return View(run);
        }
    }
}