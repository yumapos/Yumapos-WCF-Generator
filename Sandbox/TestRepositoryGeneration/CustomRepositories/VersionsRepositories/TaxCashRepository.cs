using System.Collections.Generic;
using System.Linq;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
    partial class TaxCashRepository : RepositoryBase
    {
        public TaxCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(Tax tax)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Tax tax)
        {
            throw new System.NotImplementedException();
        }

        public Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Tax> GetAll(bool? isDeleted = false)
        {
            throw new System.NotImplementedException();
        }

    }
}
