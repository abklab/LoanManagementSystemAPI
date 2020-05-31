using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoanManagementSystemAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult LoginPage()
        {
            ViewBag.Title = "404 - Not Found";

            return View("0f8fad5b-d9cb-469f-a165-70867728950e");
        }
    }

}
