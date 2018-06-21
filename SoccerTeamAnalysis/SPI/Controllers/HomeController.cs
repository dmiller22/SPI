using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Service.DataRetriever.GetPlayerInfo(19); //Only here for initial testing
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}