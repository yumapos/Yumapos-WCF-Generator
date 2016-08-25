using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration.Repositories
{
    public class RepositoryBase
    {
        protected readonly IDataAccessService DataAccessService;

        protected RepositoryBase(IDataAccessService dataAccessService)
        {
            DataAccessService = dataAccessService;
        }

    }

}