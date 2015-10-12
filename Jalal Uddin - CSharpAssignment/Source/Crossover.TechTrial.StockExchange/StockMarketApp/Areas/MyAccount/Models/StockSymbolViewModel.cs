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

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockSymbolViewModel
    {
        private readonly ILogHelperFactory _log;
        private readonly IStockAppUnitOfWork _unitOfWork;

        public StockSymbolViewModel()
        {
            _log = NinjectWebCommon.GetConcreteInstance<ILogHelperFactory>();
            _unitOfWork = NinjectWebCommon.GetConcreteInstance<IStockAppUnitOfWork>();
        }

        [Inject]
        public StockSymbolViewModel(ILogHelperFactory log, IStockAppUnitOfWork unitOfWork)
        {
            _log = log;
            _unitOfWork = unitOfWork;
        }
        
        internal object GetSymbolsJson(DataTablesAjaxRequestModel dataTablesModel)
        {
            try
            {
                Guid currentUserID = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                int pageIndex = dataTablesModel.GetPageIndex();
                int pageSize = dataTablesModel.GetPageSize();
                string sortOrder = dataTablesModel.GetSortElements(new string[] { "ID", 
                    "Ticker", "ID" });

                string searchText = dataTablesModel.GetSearchText();

                int totalRecords = 0;
                int totalDisplayableRecords = 0;

                ICollection<StockSymbol> records = new List<StockSymbol>();
                if(!string.IsNullOrWhiteSpace(searchText))
                    records = _unitOfWork.StockSymbolRepository.GetDynamic(out totalRecords, out totalDisplayableRecords,
                    x => x.UserID == currentUserID && x.Ticker.Contains(searchText), sortOrder).ToList();
                else
                    records = _unitOfWork.StockSymbolRepository.GetDynamic(out totalRecords, out totalDisplayableRecords,
                    x => x.UserID == currentUserID, sortOrder).ToList();

                totalRecords = _unitOfWork.StockSymbolRepository.GetCount(x => x.UserID == currentUserID);

                var jsonData = new
                {
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalDisplayableRecords,
                    aaData = (
                        from record in records
                        select new string[]
                        {
                            record.ID.ToString(),
                            record.Ticker,
                            record.ID.ToString()
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
    }
}