using System;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface ITaxRepository : IRepository<Tax>
    {
        Guid Insert(Tax tax);

        void UpdateByTaxId(Tax tax);

        void RemoveByTaxId(Tax tax);

        Tax GetByTaxId(int taxId, DateTimeOffset sliceDate, bool? isDeleted = false);
    }
}
