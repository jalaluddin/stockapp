using System;
namespace StockMarketSharedLibrary
{
    public interface IStockPrice
    {
        StockPrice GetStockPrice(string stockSymbol);
        System.Collections.Generic.ICollection<StockPrice> GetStockPrices(System.Collections.Generic.ICollection<string> stockSymbols);
    }
}
