using System;
namespace StockMarketApp.Areas.MyAccount.Models
{
    public interface IStockSymbolService
    {
        bool SaveStockSymbol(StockSymbol symbol);
        bool DeleteStockSymbol(System.Collections.Generic.List<Guid> ids);
    }
}
