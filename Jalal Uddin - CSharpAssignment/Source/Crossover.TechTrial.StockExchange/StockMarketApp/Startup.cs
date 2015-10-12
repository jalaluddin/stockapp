using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StockMarketApp.Startup))]
namespace StockMarketApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
