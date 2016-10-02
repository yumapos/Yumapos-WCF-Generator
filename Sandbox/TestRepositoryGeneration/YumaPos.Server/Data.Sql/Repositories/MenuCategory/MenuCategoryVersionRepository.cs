using System;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Infrastructure.DataObjects;


namespace YumaPos.Server.Data.Sql.Menu
{
    class MenuCategoryVersionRepository : RepositoryBase
    {
        public MenuCategoryVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public Guid Insert(MenuCategory menuCategory)
        {
            var InsertQuery = "INSERT INTO [History].[MenuCategoriesVersions] ([MenuCategoryId],[Name],[Modified],[ModifiedBy],[IsDeleted]) " +
                              "OUTPUT INSERTED.MenuCategoryVersionId " +
                              "VALUES (@MenuCategoryId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            return (Guid)DataAccessService.InsertObject(menuCategory, InsertQuery);
        }

    }
}
