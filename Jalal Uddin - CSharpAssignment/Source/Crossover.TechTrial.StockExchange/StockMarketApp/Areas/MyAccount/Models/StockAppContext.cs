using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockAppContext : DbContext
    {
        public StockAppContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<StockSymbol> StockSymbols { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}