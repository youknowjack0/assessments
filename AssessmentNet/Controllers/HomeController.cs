using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssessmentNet.Controllers
{
    [Authorize(Roles = "admin, testee")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}