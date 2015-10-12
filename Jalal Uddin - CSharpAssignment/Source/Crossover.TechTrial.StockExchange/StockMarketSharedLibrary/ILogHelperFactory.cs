namespace StockMarketSharedLibrary
{
    /// <summary>
    /// Base contract for Log abstract factory
    /// </summary>
    public interface ILogHelperFactory
    {
        /// <summary>
        /// Create a new ILogHelper
        /// </summary>
        /// <returns>The ILogHelper created</returns>
        ILogHelper Create();
    }
}
