using System;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
    class TaxVersionRepository : RepositoryBase
    {
        public TaxVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public Guid Insert(Tax tax)
        {
            throw new System.NotImplementedException();
        }
    }
}
