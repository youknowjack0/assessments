using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AssessmentNet.Framework;
using AssessmentNet.Models;
using AssessmentNet.ViewModels;
using AssessmentNet.ViewModels.Admin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace AssessmentNet.Controllers
{
    [Authorize(Roles = "admin")]
    public class TestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Tests/
        public ActionResult Index()
        {
            ViewBag.Message = "";
            return View(db.Tests.ToList());
        }

        // GET: /Tests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]

        public ActionResult Create([Bind(Include = "Name,MaxDurationInHours")] Test test)
        {
            test.Created = DateTime.UtcNow;
            IIdentity user = System.Web.HttpContext.Current.User.Identity;
            test.Creator = user.Name;
            test.Created = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                db.Tests.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(test);
        }

        public ActionResult EditQuestions(int id)
        {
            var test = db.Tests.Single(x => x.Id == id);
            return View(test);
        }

        public ActionResult AddQuestion(int id)
        {
            var vm = new MultiChoiceQuestionViewModel()
            {
                TestId = id
            };
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddQuestion(MultiChoiceQuestionViewModel q )
        {
            var test = db.Tests.Single(x => x.Id == q.TestId);
            var question = new MultiChoiceQuestion();
            question.AllowedTime = TimeSpan.FromMinutes(q.AllowedTimeInMinutes);
            question.QuestionHtml = q.QuestionHtml;
            question.Weight = q.Weight;
            question.Test = test;
            db.Questions.Add(question);

            if(q.Answers != null)
                foreach (var answer in q.Answers)
                {
                    var a = new MultiChoiceAnswer();
                    a.IsCorrect = answer.IsCorrect;
                    a.Question = question;
                    a.AnswerHtml = answer.AnswerHtml;
                    db.Answers.Add(a);
                }

                
            db.SaveChanges();

            return  RedirectToAction("EditQuestions", new {id = q.TestId});
        }

        public ActionResult AddAnotherAnswer()
        {
            return PartialView("_MultiChoiceAnswerRow");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult DeleteQuestion(int id)
        {
            var q = db.Questions.Single(x => x.Id == id);

            int tid = q.Test.Id;

            foreach (var response in db.Responses.Where(x => x.Question.Id == id))
            {
                if (response.TestRun.HasStarted)
                {
                    return new ContentResult(){Content = "Error: Cannot delete because a user started this test"};
                }
            }

            var set = db.Responses.Where(x => x.Question.Id == id).ToArray();
            foreach (var item in set)
            {
                db.Responses.Remove(item);
            }

            db.Questions.Remove(q);


            db.SaveChanges();

            return RedirectToAction("EditQuestions", new {id = tid});
        }

        public ActionResult TestAssignment(int id)
        {
            Test test = db.Tests.Single(x => x.Id == id);
            var y = new AssignUserToTest() {Test = test,UserEmail = null};
            return View(y);
        }

        private string CreateRandomPassword(int len)
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            const string allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ123456789!@#$%&*";
            
            for (int i = 0; i < len; i++)
            {
                sb.Append(allowed[r.Next()%allowed.Length]);
            }
            return sb.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTest([Bind(Include = "UserEmail, TestId")] AssignUserToTest model)
        {
            var email = model.UserEmail.ToLower().Trim();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager) {AllowOnlyAlphanumericUserNames = false};
            ApplicationUser acct = userManager.FindByName(email);

            string password = "<user already has an account>";
            if (acct == null)
            {
                password = CreateRandomPassword(16);
                var x = userManager.Create(new ApplicationUser(email), password);
                acct = userManager.FindByName(email);
                userManager.AddToRole(acct.Id, "testee");
            }

            var test = db.Tests.Single(x => x.Id == model.TestId);

            if (db.TestRun.Any(x => x.Test.Id == test.Id && x.Testee.Id == acct.Id))
            {
                ViewBag.Message = string.Format("User {0} already has a test run for: \"{1}\"", acct.UserName, test.Name);
                return View("Index", db.Tests.ToList());
            }

            TestRun run = new TestRun()
            {
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow + TimeSpan.FromHours(test.MaxDurationInHours),
                HasStarted = false,
                Test = test,
                Testee = acct
            };

            db.TestRun.Add(run);

            foreach (var item in test.Questions)
            {
                db.Responses.Add(new QuestionResponse()
                {
                    Question = item,
                    TestRun = run,
                });
            }

            db.SaveChanges();

            ViewBag.Message = string.Format("Assigned user {0} to test, with password: \"{1}\"", acct.UserName, password);
            return View("Index", db.Tests.ToList());
        }

        //test id
        public ActionResult Results(int id)
        {
            var results = db.Responses.Where(x => x.TestRun.Test.Id == id).GroupBy(x => x.TestRun).ToArray();

            return View(results);
        }
        
        public ActionResult EditQuestion(int id)
        {
            var question = (MultiChoiceQuestion) db.Questions.Single(x => x.Id == id);

            var vm = new MultiChoiceQuestionViewModel()
            {
                AllowedTimeInMinutes = question.AllowedTime.TotalMinutes,
                Answers = question.Answers.Select(x => new SimpleAnswerViewModel() {AnswerHtml = x.AnswerHtml, AnswerId = x.Id, IsCorrect = x.IsCorrect}),
                QuestionHtml = question.QuestionHtml,
                QuestionId = question.Id,
                TestId = question.Test.Id,
                Weight = question.Weight
            };

            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditQuestion(MultiChoiceQuestionViewModel vm)
        {
            var question = (MultiChoiceQuestion)db.Questions.Single(x => x.Id == vm.QuestionId);

            question.AllowedTime = TimeSpan.FromMinutes(vm.AllowedTimeInMinutes);
            question.QuestionHtml = vm.QuestionHtml;

            foreach (var answer in vm.Answers)
            {
                var dba = (MultiChoiceAnswer) db.Answers.FirstOrDefault(x => x.Id == answer.AnswerId);
                if (dba == null)
                {
                    db.Answers.Add(new MultiChoiceAnswer() {AnswerHtml = answer.AnswerHtml, IsCorrect = answer.IsCorrect, Question = question});
                }
                else
                {
                    dba.AnswerHtml = answer.AnswerHtml;
                    dba.IsCorrect = answer.IsCorrect;
                    db.Entry(dba).State= EntityState.Modified;
                }
            }

            foreach (var answer in db.Answers.OfType<MultiChoiceAnswer>().Where(x => x.Question.Id == question.Id).ToList())
            {
                if (!vm.Answers.Any(x => x.AnswerId == answer.Id))
                {
                    db.Answers.Remove(answer);
                }
            }

            db.SaveChanges();

            return View(vm);
        }
    }
}
