using System;

namespace StockMarketSharedLibrary
{
    [Serializable]
    public enum LogType
    {
        HandledLog,
        UnhandledLog,
        DBLog
    }

    public interface ILogHelper
    {
        void WriteLog(string className, string methodName, string message);
        void WriteLog(LogType type, string className, string methodName, Exception ex, string message);
    }
}
