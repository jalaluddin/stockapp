using System;
namespace StockMarketSharedLibrary
{
    public interface IHashHelper
    {
        string ComputeHash(string message, string key);
    }
}
