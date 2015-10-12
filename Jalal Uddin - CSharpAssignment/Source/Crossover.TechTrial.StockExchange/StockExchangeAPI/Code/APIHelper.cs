using Newtonsoft.Json;
using StockMarketSharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockExchangeAPI
{
    public class APIHelper : IAPIHelper
    {
        private readonly IHashHelper _hashHelper;
        private readonly IStockDataUnitOfWork _unitOfWork;

        public APIHelper(IStockDataUnitOfWork unitOfWork, IHashHelper hashHelper)
        {
            _hashHelper = hashHelper;
            _unitOfWork = unitOfWork;
        }

        protected virtual string SerializeStockCodes(List<string> stockCodes)
        {
            return JsonConvert.SerializeObject(stockCodes);
        }

        public virtual bool ValidateRequest(string publicKey, string hash, List<string> stockCodes)
        {
            var privateKey = GetPrivateKey(publicKey);
            if (privateKey != null)
            {
                string computedHash = _hashHelper.ComputeHash(SerializeStockCodes(stockCodes), privateKey);
                return hash == computedHash;
            }
            else
                return false;
        }

        protected virtual string GetPrivateKey(string publicKey)
        {
            var apiKeys = _unitOfWork.UserAPIRecordsRepository.Get(x => x.PublicKey == publicKey).FirstOrDefault();
            if (apiKeys != null)
                return apiKeys.PrivateKey;
            else
                return null;
        }
    }
}
