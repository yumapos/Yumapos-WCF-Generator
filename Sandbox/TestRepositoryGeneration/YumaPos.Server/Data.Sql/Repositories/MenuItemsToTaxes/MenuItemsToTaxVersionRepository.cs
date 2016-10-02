using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Data.Sql.Menu
{
    class MenuItem2TaxVersionRepository : RepositoryBase
    {
        public MenuItem2TaxVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public void Insert(MenuItems2Taxes mt)
        {
            var InsertQuery = "INSERT INTO [History].[MenuItemsToTaxesVersions] ([ItemId],[ItemVersionId],[Modified],[TaxId],[TaxVersionId])" +
                              "VALUES (@ItemId, @ItemVersionId, @Modified, @TaxId, @TaxVersionId )";

            DataAccessService.InsertObject(mt, InsertQuery);
        }
    }
}
