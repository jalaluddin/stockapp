using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace StockExchangeAPI
{
    public partial class StockDataContext : DbContext
    {
        public StockDataContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<UserAPIRecords> UserAPIRecords { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
