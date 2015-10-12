using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class StockSymbol : Entity
    {
        [Required]
        [MaxLength(10)]
        public string Ticker { get; set; }
        public Guid UserID { get; set; }
    }
}