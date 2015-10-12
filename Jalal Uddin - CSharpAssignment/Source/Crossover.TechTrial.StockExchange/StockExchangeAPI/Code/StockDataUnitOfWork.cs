using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchangeAPI
{
    public class StockDataUnitOfWork : IDisposable, IStockDataUnitOfWork
    {
        private StockDataContext _context;
        private IDbCommandExecutionService _dbCommandExecutionService;
        private IDbCommandFactory _dbCommandFactory;

        public StockDataUnitOfWork(StockDataContext context, IDbCommandExecutionService dbCommandExecutionService, IDbCommandFactory dbCommandFactory)
        {
            _context = context;
            _dbCommandExecutionService = dbCommandExecutionService;
            _dbCommandFactory = dbCommandFactory;
        }

        private GenericRepository<UserAPIRecords> userAPIRecordsRepository;

        public GenericRepository<UserAPIRecords> UserAPIRecordsRepository
        {
            get
            {
                if (this.userAPIRecordsRepository == null)
                {
                    this.userAPIRecordsRepository = new GenericRepository<UserAPIRecords>(_context, _dbCommandExecutionService, _dbCommandFactory);
                }
                return userAPIRecordsRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
