using Ninject;
using StockMarketApp.App_Start;
using StockMarketApp.Models;
using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using StockMarketApp.StockDataService;
using Newtonsoft.Json;
using System.Configuration;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockPriceViewModel
    {
        private readonly ILogHelperFactory _log;
        private readonly IStockAppUnitOfWork _unitOfWork;

        public StockPriceViewModel()
        {
            _log = NinjectWebCommon.GetConcreteInstance<ILogHelperFactory>();
            _unitOfWork = NinjectWebCommon.GetConcreteInstance<IStockAppUnitOfWork>();
        }

        [Inject]
        public StockPriceViewModel(ILogHelperFactory log, IStockAppUnitOfWork unitOfWork)
        {
            _log = log;
            _unitOfWork = unitOfWork;
        }
        
        internal object GetPricesJson(DataTablesAjaxRequestModel dataTablesModel)
        {
            try
            {
                Guid currentUserID = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                int pageIndex = dataTablesModel.GetPageIndex();
                int pageSize = dataTablesModel.GetPageSize();
                string sortOrder = dataTablesModel.GetSortElements(new string[] { "ID", 
                    "Ticker", "Price", "ID" });

                string searchText = dataTablesModel.GetSearchText();

                int totalRecords = 0;
                int totalDisplayableRecords = 0;

                ICollection<StockSymbol> stockSymbols = new List<StockSymbol>();
                if(!string.IsNullOrWhiteSpace(searchText))
                    stockSymbols = _unitOfWork.StockSymbolRepository.GetDynamic(out totalRecords, out totalDisplayableRecords,
                    x => x.UserID == currentUserID && x.Ticker.Contains(searchText), sortOrder).ToList();
                else
                    stockSymbols = _unitOfWork.StockSymbolRepository.GetDynamic(out totalRecords, out totalDisplayableRecords,
                    x => x.UserID == currentUserID, sortOrder).ToList();

                totalRecords = _unitOfWork.StockSymbolRepository.GetCount(x => x.UserID == currentUserID);

                if(stockSymbols == null || stockSymbols.Count == 0)
                    return DataTablesAjaxRequestModel.EmptyResult;

                ICollection<StockPrice> records = GetPriceFromAPI(stockSymbols);
                var jsonData = new
                {
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalDisplayableRecords,
                    aaData = (
                        from record in records
                        select new string[]
                        {
                            record.Symbol,
                            record.Price.ToString()
                        }
                    ).ToArray()
                };

                return jsonData;
            }
            catch(Exception ex)
            {
                _log.Create().WriteLog(LogType.HandledLog, this.GetType().Name, "SaveStockSymbol", ex, "Failed to add stock symbol");

                UserSession.ActionResponseMessage = new ActionResponse("Failed to get symbol list", ActionResponseMessageType.Error);
            }
            return DataTablesAjaxRequestModel.EmptyResult;
        }

        public List<StockPrice> GetPriceFromAPI(ICollection<StockSymbol> symbols)
        {
            if(symbols != null && symbols.Count > 0)
            {
                StockDataSoapClient client = new StockDataSoapClient();
                var request = new StockPriceRequest();
                request.Body = new StockPriceRequestBody();
                request.Body.pubicKey = ConfigurationManager.AppSettings["PublicKey"];
                request.Body.stockCodes = new ArrayOfString();

                foreach (var symbol in symbols)
                {

                    request.Body.stockCodes.Add(symbol.Ticker);
                }

                request.Body.hash = new HashHelper().ComputeHash(JsonConvert.SerializeObject(request.Body.stockCodes), 
                    ConfigurationManager.AppSettings["PrivateKey"]);

                var response = client.StockPrice(request);
                List<StockPrice> prices = JsonConvert.DeserializeObject<List<StockPrice>>(response.Body.StockPriceResult);
                return prices;
            }
            return null;
        }
    }
}