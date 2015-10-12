using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockMarketSharedLibrary
{
    public class Log4NetLogHelperFactory : ILogHelperFactory
    {
        #region ILogHelperFactory Members

        public Log4NetLogHelperFactory()
        {
            log4net.Config.XmlConfigurator.Configure();
            LogHelperFactory.SetCurrent(this);
        }

        public Log4NetLogHelperFactory(string configFilePath)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configFilePath));
            LogHelperFactory.SetCurrent(this);
        }

        public ILogHelper Create()
        {
            return new Log4NetLogHelper();
        }

        #endregion
    }
}
