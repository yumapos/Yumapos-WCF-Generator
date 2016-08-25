using System.Configuration;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration
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