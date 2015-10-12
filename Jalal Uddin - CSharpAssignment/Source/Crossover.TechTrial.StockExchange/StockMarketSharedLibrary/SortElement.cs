using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    public class SortElement
    {
        public string ColumnName { get; set; }
        public SortOrder Order { get; set; }

        public SortElement(string columnName, SortOrder order)
        {
            this.ColumnName = columnName;
            this.Order = order;
        }
    }
}
