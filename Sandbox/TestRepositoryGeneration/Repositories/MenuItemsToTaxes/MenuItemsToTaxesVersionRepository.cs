using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Data.Sql.Menu
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
