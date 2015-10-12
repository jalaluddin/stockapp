using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockSymbolService : IStockSymbolService
    {
        private readonly ILogHelperFactory _log;
        private readonly IStockAppUnitOfWork _unitOfWork;

        public StockSymbolService(ILogHelperFactory log, IStockAppUnitOfWork unitOfWork)
        {
            _log = log;
            _unitOfWork = unitOfWork;
        }

        public bool SaveStockSymbol(StockSymbol symbol)
        {
            try
            {
                if (symbol == null)
                    throw new ArgumentNullException("Symbol cannot be null");
                if (string.IsNullOrWhiteSpace(symbol.Ticker) || symbol.Ticker.Length > 10)
                    throw new ArgumentException("Ticker should be 1 to 10 character long");

                Guid currentUserID = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                symbol.UserID = currentUserID;

                _unitOfWork.StockSymbolRepository.Insert(symbol);
                _unitOfWork.Save();

                return true;
            }
            catch(Exception ex)
            {
                _log.Create().WriteLog(LogType.HandledLog, this.GetType().Name, "SaveStockSymbol", ex, "Failed to add stock symbol");
            }
            return false;
        }

        public bool DeleteStockSymbol(List<Guid> ids)
        {
            try
            {
                if (ids == null)
                    throw new ArgumentNullException("ID cannot be null");

                foreach(var id in ids)
                    _unitOfWork.StockSymbolRepository.Delete(id);

                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                _log.Create().WriteLog(LogType.HandledLog, this.GetType().Name, "DeleteStockSymbol", ex, "Failed to delete stock symbol");
            }
            return false;
        }
    }
}