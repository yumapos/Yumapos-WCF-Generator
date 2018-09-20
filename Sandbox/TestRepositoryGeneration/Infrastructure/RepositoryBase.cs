namespace TestRepositoryGeneration.Infrastructure
{
    public class RepositoryBase
    {
        protected readonly IDataAccessService DataAccessService;
        protected readonly IDataAccessController DataAccessController;

        protected const int MaxRepositoryParams = 2000;

        protected RepositoryBase(IDataAccessService dataAccessService)
        {
            DataAccessService = dataAccessService;
        }

        protected RepositoryBase(IDataAccessService dataAccessService,
            IDataAccessController dataAccessController)
        {
            DataAccessService = dataAccessService;
            DataAccessController = dataAccessController;
        }
    }

}