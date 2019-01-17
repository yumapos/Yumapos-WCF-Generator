namespace TestRepositoryGeneration.Infrastructure
{
    public class RepositoryBase
    {
        protected readonly IDataAccessService DataAccessService;
        protected readonly IDataAccessController DataAccessController;

        protected const int MaxRepositoryParams = 2000;
        protected const int MaxInsertManyRows = 1000;
        protected const bool ClearCache = true;
        protected const bool CheckConstraintAfterInsertMany = true;

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