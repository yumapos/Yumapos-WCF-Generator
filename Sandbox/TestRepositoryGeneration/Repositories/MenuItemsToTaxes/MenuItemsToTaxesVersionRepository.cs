using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuItemsToTaxesVersionRepository : RepositoryBase
    {
        public MenuItemsToTaxesVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public void Insert(MenuItemToTax mt)
        {
            var InsertQuery = "INSERT INTO [History].[MenuItemsToTaxesVersions] ([ItemId],[ItemVersionId],[Modified],[TaxId],[TaxVersionId])" +
                              "VALUES (@ItemId, @ItemVersionId, @Modified, @TaxId, @TaxVersionId )";

            DataAccessService.InsertObject(mt, InsertQuery);
        }
    }
}
