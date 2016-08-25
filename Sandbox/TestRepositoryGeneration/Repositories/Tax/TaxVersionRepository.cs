using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;

namespace VersionedRepositoryGeneration.Repositories.Tax
{
    class TaxVersionRepository : RepositoryBase
    {
        public TaxVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public Guid Insert(Models.Tax tax)
        {
            var InsertQuery = "INSERT INTO [History].[TaxesVersions] ([TaxId],[Name],[Modified],[ModifiedBy],[IsDeleted]) " +
                              "OUTPUT INSERTED.TaxVersionId " +
                              "VALUES (@TaxId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            return (Guid)DataAccessService.InsertObject(tax, InsertQuery);
        }
    }
}
