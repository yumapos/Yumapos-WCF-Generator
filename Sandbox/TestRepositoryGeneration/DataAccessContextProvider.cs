using System.Configuration;
using YumaPos.FrontEnd.Data.Infrastructure;

namespace YumaPos.FrontEnd.WPF.Common.Data
{
    public class DataAccessContextProvider : IDataAccessContextProvider
    {
        private string _connection;

        public string Connection
        {
            get
            {
                if (string.IsNullOrEmpty(_connection))
                {
                    _connection = ConfigurationManager.ConnectionStrings["VersioningDataBase"].ConnectionString;
                }

                return _connection;
            }
        }
    }
}