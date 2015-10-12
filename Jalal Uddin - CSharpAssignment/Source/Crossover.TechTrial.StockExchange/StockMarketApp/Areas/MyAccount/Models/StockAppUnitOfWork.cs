using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockAppUnitOfWork : IDisposable, IStockAppUnitOfWork
    {
        private StockAppContext _context;
        private IDbCommandExecutionService _dbCommandExecutionService;
        private IDbCommandFactory _dbCommandFactory;

        public StockAppUnitOfWork(StockAppContext context, IDbCommandExecutionService dbCommandExecutionService, IDbCommandFactory dbCommandFactory)
        {
            _context = context;
            _dbCommandExecutionService = dbCommandExecutionService;
            _dbCommandFactory = dbCommandFactory;
        }

        private GenericRepository<StockSymbol> stockSymbolRepository;

        public GenericRepository<StockSymbol> StockSymbolRepository
        {
            get
            {
                if (this.stockSymbolRepository == null)
                {
                    this.stockSymbolRepository = new GenericRepository<StockSymbol>(_context, _dbCommandExecutionService, _dbCommandFactory);
                }
                return stockSymbolRepository;
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