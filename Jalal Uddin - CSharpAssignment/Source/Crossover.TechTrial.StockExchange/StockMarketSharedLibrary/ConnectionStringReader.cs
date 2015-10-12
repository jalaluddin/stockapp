using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarketSharedLibrary
{
    public class ConnectionStringReader
    {
        private string _connectionStringName;
        public ConnectionStringReader()
        {
            _connectionStringName = ConfigurationManager.AppSettings["CurrentConnectionStringName"];
        }

        public ConnectionStringReader(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        private static string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    if (!(string.IsNullOrEmpty(_connectionStringName) || string.IsNullOrWhiteSpace(_connectionStringName)))
                    {
                        ConnectionStringSettings connString = ConfigurationManager.ConnectionStrings[_connectionStringName];
                        if (connString != null)
                        {
                            _connectionString = connString.ConnectionString;
                            return _connectionString;
                        }
                        else
                            throw new ConfigurationErrorsException("Connection string is not found with connection string name: CurrentConnectionStringName");
                    }
                    else
                        throw new ConfigurationErrorsException("CurrentConnectionStringName is not found in AppSettings");
                }
                return _connectionString;
            }
        }
    }
}
