using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StockMarketSharedLibrary
{
    public class Log4netExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            LogHelperFactory.CreateLog().WriteLog(LogType.UnhandledLog, this.ToString(),
                "OnException", context.Exception, "Unhandled Error in Web application: Web");
        }
    }
}