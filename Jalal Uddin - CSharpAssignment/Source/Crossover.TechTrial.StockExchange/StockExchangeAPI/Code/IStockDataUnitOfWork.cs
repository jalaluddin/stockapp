using StockMarketSharedLibrary;
using System;
namespace StockExchangeAPI
{
    public interface IStockDataUnitOfWork
    {
        GenericRepository<UserAPIRecords> UserAPIRecordsRepository { get; }
        void Save();
        void Dispose();
    }
}
