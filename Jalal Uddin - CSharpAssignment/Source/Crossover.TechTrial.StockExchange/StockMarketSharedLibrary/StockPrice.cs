using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    [Serializable]
    public class StockPrice : IStockPrice
    {
        public string Symbol { get; set; }
        public int Price { get; set; }

        private readonly Random _random;

        public StockPrice()
        {
            _random = new Random(DateTime.UtcNow.TimeOfDay.Milliseconds);
        }

        public virtual StockPrice GetStockPrice(string stockSymbol)
        {
            return new StockPrice() { Symbol = stockSymbol, Price = _random.Next(1, 1000) }; 
        }

        public virtual ICollection<StockPrice> GetStockPrices(ICollection<string> stockSymbols)
        {
            List<StockPrice> prices = new List<StockPrice>();
            foreach(var symbol in stockSymbols)
            {
                prices.Add(GetStockPrice(symbol));
            }
            return prices;
        }
    }
}
