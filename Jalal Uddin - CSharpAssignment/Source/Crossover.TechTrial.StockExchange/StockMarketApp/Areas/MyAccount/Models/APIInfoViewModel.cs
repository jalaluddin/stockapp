using Ninject;
using StockMarketApp.App_Start;
using StockMarketApp.Models;
using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Configuration;

namespace StockMarketApp.Areas.MyAccount.Models
{
    public class APIInfoViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage="Public key should be at most 50 character long")]
        [Display(Name = "Public Key")]
        public string PublicKey { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Private key should be at most 50 character long")]
        [Display(Name = "Private Key")]
        public string PrivateKey { get; set; }

        private readonly ILogHelperFactory _log;

        public APIInfoViewModel()
        {
            _log = NinjectWebCommon.GetConcreteInstance<ILogHelperFactory>();
        }

        [Inject]
        public APIInfoViewModel(ILogHelperFactory log)
        {
            _log = log;
        }

        internal void LoadAPIKeys()
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("/");

                this.PublicKey = config.AppSettings.Settings["PublicKey"].Value;
                this.PrivateKey = config.AppSettings.Settings["PrivateKey"].Value;
            }
            catch (Exception ex)
            {
                _log.Create().WriteLog(LogType.HandledLog, this.GetType().Name, "LoadAPIKeys", ex, "Failed to load api keys");
                UserSession.ActionResponseMessage = new ActionResponse("Failed to load api keys", ActionResponseMessageType.Error);
            }
        }

        internal void UpdateAPIKeys()
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
                
                config.AppSettings.Settings["PublicKey"].Value = this.PublicKey;
                config.AppSettings.Settings["PrivateKey"].Value = this.PrivateKey;
                config.Save(ConfigurationSaveMode.Modified);

                UserSession.ActionResponseMessage = new ActionResponse("API keys update complete", ActionResponseMessageType.Success);
            }
            catch(Exception ex)
            {
                _log.Create().WriteLog(LogType.HandledLog, this.GetType().Name, "UpdateAPIKeys", ex, "Failed to update api keys");
                UserSession.ActionResponseMessage = new ActionResponse("Failed to update api keys", ActionResponseMessageType.Error);
            }
        }
    }
}