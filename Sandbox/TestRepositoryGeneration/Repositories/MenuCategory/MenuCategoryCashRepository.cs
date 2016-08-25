using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuCategoryCashRepository : RepositoryBase
    {
        public MenuCategoryCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(Models.MenuCategory menuCategory)
        {
            var InsertQuery = "INSERT INTO [dbo].[MenuCategories] ([MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted]) VALUES " +
                              "(@MenuCategoryId, @MenuCategoryVersionId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            DataAccessService.InsertObject(menuCategory, InsertQuery);
        }

        public void Update(Models.MenuCategory menuCategory)
        {
            var UpdateQuery = "UPDATE [dbo].[MenuCategories] SET [MenuCategoryVersionId] = @MenuCategoryVersionId, [Name] = @Name," +
                              "[Modified] = @Modified, [ModifiedBy] = @ModifiedBy,[IsDeleted] = @IsDeleted WHERE [MenuCategoryId] = @MenuCategoryId";

            DataAccessService.PersistObjectAsync(menuCategory, UpdateQuery);
        }

        public Models.MenuCategory GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[MenuCategories] Where [MenuCategoryId] = @menuCategoryId";

            var result = DataAccessService.Get<Models.MenuCategory>(SelectQuery, new { menuCategoryId });
            return result.FirstOrDefault();
        }

        public IEnumerable<Models.MenuCategory> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuCategoryId],[MenuCategoryVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[MenuCategories]";

            var result = DataAccessService.Get<Models.MenuCategory>(SelectQuery, null);
            return result.ToList();
        }

    }
}
