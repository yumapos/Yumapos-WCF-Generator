namespace TestRepositoryGeneration.Infrastructure
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