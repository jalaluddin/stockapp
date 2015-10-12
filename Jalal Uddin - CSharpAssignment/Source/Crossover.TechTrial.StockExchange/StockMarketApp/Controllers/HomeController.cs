using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockMarketApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Stock Market Expert - Intelligent business analysis tool for stock market.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "We are happy to hear from you!";

            return View();
        }
    }
}