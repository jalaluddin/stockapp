using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchangeAPI
{
    public class UserAPIRecords : Entity
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public Guid UserID { get; set; }
    }
}
