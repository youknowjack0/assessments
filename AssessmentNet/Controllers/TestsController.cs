﻿using System;
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
        public ActionResult AddQuestion(MultiChoiceQuestionViewModel q )
        {
            if (ModelState.IsValid)
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

            return View(q);
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
            var y = new AssignUserToTest() {Test = test, UserEmail = null};
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
        public ActionResult AssignTest([Bind(Include = "UserEmail, Test")] AssignUserToTest model)
        {
            var email = model.UserEmail.ToLower().Trim();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser acct = userManager.FindByName(email);

            string password = "<user already has an account>";
            if (acct == null)
            {
                password = CreateRandomPassword(16);
                var x = userManager.Create(new ApplicationUser(email), password);
                acct = userManager.FindByName(email);
            }

            var test = db.Tests.Single(x => x.Id == model.Test.Id);

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
                    HasStarted = false,
                    HasFinished = false,
                    Question = item,
                    TestRun = run,
                });
            }

            db.SaveChanges();

            ViewBag.Message = string.Format("Assigned user {0} to test, with password: \"{1}\"", acct.UserName, password);
            return View("Index", db.Tests.ToList());
        }
    }
}
