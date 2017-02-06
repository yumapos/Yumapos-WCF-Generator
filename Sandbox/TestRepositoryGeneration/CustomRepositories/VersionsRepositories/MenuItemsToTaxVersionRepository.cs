using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
    class MenuItem2TaxVersionRepository : RepositoryBase
    {
        public MenuItem2TaxVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public void Insert(MenuItems2Taxes mt)
        {
            throw new System.NotImplementedException();
        }
    }
}
