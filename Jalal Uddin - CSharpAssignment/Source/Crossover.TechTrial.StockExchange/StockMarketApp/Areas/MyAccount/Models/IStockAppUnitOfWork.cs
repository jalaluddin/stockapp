using StockMarketSharedLibrary;
using System;
namespace StockMarketApp.Areas.MyAccount.Models
{
    public interface IStockAppUnitOfWork
    {
        void Dispose();
        void Save();
        GenericRepository<StockSymbol> StockSymbolRepository { get; }
    }
}
