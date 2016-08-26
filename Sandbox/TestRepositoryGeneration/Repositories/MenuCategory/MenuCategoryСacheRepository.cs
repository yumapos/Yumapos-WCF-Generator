using System.Collections.Generic;
using System.Linq;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Data.Sql.Menu
{
    class MenuCategoryСacheRepository : RepositoryBase
    {
        public MenuCategoryСacheRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(MenuCategory menuCategory)
        {
            var InsertQuery = "INSERT INTO [dbo].[MenuCategories] ([MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted]) VALUES " +
                              "(@MenuCategoryId, @MenuCategoryVersionId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            DataAccessService.InsertObject(menuCategory, InsertQuery);
        }

        public void Update(MenuCategory menuCategory)
        {
            var UpdateQuery = "UPDATE [dbo].[MenuCategories] SET [MenuCategoryVersionId] = @MenuCategoryVersionId, [Name] = @Name," +
                              "[Modified] = @Modified, [ModifiedBy] = @ModifiedBy,[IsDeleted] = @IsDeleted WHERE [MenuCategoryId] = @MenuCategoryId";

            DataAccessService.PersistObjectAsync(menuCategory, UpdateQuery);
        }

        public MenuCategory GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[MenuCategories] Where [MenuCategoryId] = @menuCategoryId";

            var result = DataAccessService.Get<MenuCategory>(SelectQuery, new { menuCategoryId });
            return result.FirstOrDefault();
        }

        public IEnumerable<MenuCategory> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[MenuCategories]";

            var result = DataAccessService.Get<MenuCategory>(SelectQuery, null);
            return result.ToList();
        }

    }
}
