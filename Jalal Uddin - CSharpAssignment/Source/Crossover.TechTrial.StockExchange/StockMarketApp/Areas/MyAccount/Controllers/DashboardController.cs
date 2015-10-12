using Newtonsoft.Json;
using Ninject;
using StockMarketApp.Areas.MyAccount.Models;
using StockMarketApp.StockDataService;
using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockMarketApp.Areas.MyAccount.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStockSymbolService _stockSymbolService;

        [Inject]
        public DashboardController(IStockSymbolService stockSymbolService)
        {
            _stockSymbolService = stockSymbolService;
        }

        public ActionResult StockPrices()
        {
            return View();
        }

        public ActionResult StockSymbols()
        {
            return View();
        }

        public ActionResult APIInfo()
        {
            APIInfoViewModel model = new APIInfoViewModel();
            model.LoadAPIKeys();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult APIInfo(APIInfoViewModel model)
        {
            if (ModelState.IsValid)
                model.UpdateAPIKeys();

            return View(model);
        }

        public JsonResult GridDataForSymbols(DataTablesAjaxRequestModel dataTablesModel)
        {
            StockSymbolViewModel model = new StockSymbolViewModel();
            var jsonData = model.GetSymbolsJson(dataTablesModel);

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDataForStockPrices(DataTablesAjaxRequestModel dataTablesModel)
        {
            StockPriceViewModel model = new StockPriceViewModel();
            var jsonData = model.GetPricesJson(dataTablesModel);

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddStockSymbol()
        {
            StockSymbol symbol = new StockSymbol();
            return View(symbol);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddStockSymbol(StockSymbol symbol)
        {
            if (ModelState.IsValid && _stockSymbolService.SaveStockSymbol(symbol))
                return RedirectToAction("StockSymbols");

            return View(symbol);
        }

        [HttpPost]
        public ActionResult DeleteStockSymbol(List<Guid> ids)
        {
            if (ModelState.IsValid && _stockSymbolService.DeleteStockSymbol(ids))
                return RedirectToAction("StockSymbols");

            StockSymbol symbol = new StockSymbol();
            return View(symbol);
        }
    }
}