using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockMarketSharedLibrary
{
    [Serializable]
    public enum ActionResponseMessageType
    {
        Error,
        Success,
        Info,
        Warning
    }

    [Serializable]
    public class ActionResponse
    {
        public string Message
        {
            get;
            set;
        }

        public ActionResponseMessageType MessageType
        {
            get;
            set;
        }

        public ActionResponse(string message, ActionResponseMessageType type)
        {
            this.Message = message;
            this.MessageType = type;
        }
    }
}