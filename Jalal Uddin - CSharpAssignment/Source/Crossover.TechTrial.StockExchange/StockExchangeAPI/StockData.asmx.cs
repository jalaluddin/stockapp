using Newtonsoft.Json;
using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace StockExchangeAPI
{
    /// <summary>
    /// Returns stock prices for specific stock codes
    /// </summary>
    [WebService(Namespace = "http://www.stockdataexpert.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class StockData : System.Web.Services.WebService
    {
        private readonly IStockPrice _stockPrice;
        private readonly IAPIHelper _apiHelper;
        private const string _invalidRequest = "Authentication Failed - Bad Data";
        public StockData()
        {
            _stockPrice = NinjectWebCommon.GetConcreteInstance<IStockPrice>();
            _apiHelper = NinjectWebCommon.GetConcreteInstance<IAPIHelper>();
        }

        [WebMethod]
        public string StockPrice(string pubicKey, string hash, List<string> stockCodes)
        {
            if (string.IsNullOrWhiteSpace(pubicKey) || string.IsNullOrWhiteSpace(hash) 
                || !_apiHelper.ValidateRequest(pubicKey, hash, stockCodes))
                throw new InvalidOperationException(_invalidRequest);

            return JsonConvert.SerializeObject(_stockPrice.GetStockPrices(stockCodes));
        }
    }
}
