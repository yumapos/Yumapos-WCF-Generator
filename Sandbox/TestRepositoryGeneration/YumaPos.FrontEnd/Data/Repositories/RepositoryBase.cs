using YumaPos.FrontEnd.Infrastructure.Configuration;

namespace YumaPos.FrontEnd.Data.Repositories
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