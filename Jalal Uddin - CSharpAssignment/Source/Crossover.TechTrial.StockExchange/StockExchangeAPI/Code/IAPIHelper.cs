using System;
namespace StockExchangeAPI
{
    public interface IAPIHelper
    {
        bool ValidateRequest(string publicKey, string hash, System.Collections.Generic.List<string> stockCodes);
    }
}
