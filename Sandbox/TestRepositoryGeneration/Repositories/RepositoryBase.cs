using YumaPos.FrontEnd.Infrastructure.Configuration;

namespace YumaPos.Server.Data.Sql
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