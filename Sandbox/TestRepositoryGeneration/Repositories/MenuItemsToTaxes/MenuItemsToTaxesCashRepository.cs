using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuItemsToTaxesCashRepository : RepositoryBase
    {
        public MenuItemsToTaxesCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public void Insert(MenuItemToTax mt)
        {
            var InsertQuery = "INSERT INTO [dbo].[MenuItemsToTaxes] ([ItemId],[TaxId]) VALUES (@itemId, @taxId)";

            DataAccessService.InsertObject(mt, InsertQuery);
        }

        public Guid GetMenuItemIdByTaxId(Guid taxId)
        {
            var SelectQuery = "SELECT [MenuItemId] FROM [dbo].[Taxes] Where [TaxId] = @taxId";

            var result = DataAccessService.Get<Guid>(SelectQuery, new { taxId });
            return result.FirstOrDefault();
        }

        public void RemoveByMenuItemId(Guid itemId)
        {
            var RemoveQuery = "DELETE FROM [dbo].[MenuItemsToTaxes] WHERE [ItemId] = @ItemId";

            DataAccessService.ExecuteAsync(RemoveQuery, new { itemId });
        }

        public void RemoveByTaxId(Guid taxId)
        {
            var RemoveQuery = "DELETE FROM [dbo].[MenuItemsToTaxes] WHERE [TaxId] = @taxId";

            DataAccessService.ExecuteAsync(RemoveQuery, new { taxId });
        }

        public IEnumerable<Guid> GetTaxeIdsByMenuItemId(System.Guid itemId)
        {
            var SelectQuery = "SELECT [TaxId] FROM [dbo].[MenuItemsToTaxes] Where [ItemId] = @ItemId";

            var result = DataAccessService.Get<Guid>(SelectQuery, new { itemId });
            return result.ToList();
        }

        public IEnumerable<Guid> GetMenuItemIdsByTaxId(System.Guid taxId)
        {
            var SelectQuery = "SELECT [ItemId] FROM [dbo].[MenuItemsToTaxes] Where [TaxId] = @taxId";

            var result = DataAccessService.Get<Guid>(SelectQuery, new { taxId });
            return result.ToList();
        }
    }
}
