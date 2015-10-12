using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockMarketApp.Models
{
    public enum ResponseType
    {
        none,
        error,
        success
    }

    public static class UserSession
    {
        private enum SessionKeys
        {
            ActionResponseMessage
        }

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public static ActionResponse ActionResponseMessage
        {
            get
            {
               var result = (ActionResponse)HttpContext.Current.Session[SessionKeys.ActionResponseMessage.ToString()];
               HttpContext.Current.Session[SessionKeys.ActionResponseMessage.ToString()] = null;
               return result;
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.ActionResponseMessage.ToString()] = value;
            }
        }
    }
}