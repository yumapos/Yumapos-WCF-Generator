using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuItemCashRepository : RepositoryBase
    {
        public MenuItemCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public void Insert(Models.MenuItem menuItem)
        {
            var InsertQuery = "DECLARE @TempPKTable TABLE (ItemId uniqueidentifier); " +
                              "DECLARE @TempPKItemId uniqueidentifier; " +
                              "INSERT INTO [dbo].[RecipieItems] ([ItemId],[ItemVersionId],[IsDeleted]) " +
                              "OUTPUT INSERTED.ItemId INTO @TempPKTable " +
                              "VALUES (@ItemId, @ItemVersionId, @IsDeleted);" +
                              "SELECT @TempPKItemId = ItemId FROM @TempPKTable " +
                              "INSERT INTO [dbo].[MenuItems] ([ItemId],[ItemVersionId],[Name],[Modified],[ModifiedBy],[MenuCategoryId],[IsDeleted]) " +
                              "VALUES (@ItemId, @ItemVersionId, @Name, @Modified, @ModifiedBy, @MenuCategoryId, @IsDeleted)";

            DataAccessService.InsertObject(menuItem, InsertQuery);
        }

        public void Update(Models.MenuItem menuItem)
        {
            var UpdateQuery = "UPDATE [dbo].[MenuItems] SET [ItemVersionId] = @ItemVersionId, [Name] = @Name," +
                              "[Modified] = @Modified, [ModifiedBy] = @ModifiedBy, [MenuCategoryId] = @MenuCategoryId, [IsDeleted] = @IsDeleted " +
                              "WHERE [ItemId] = @ItemId" +
                              "UPDATE [dbo].[RecipieItems] SET [ItemVersionId] = @ItemVersionId, [IsDeleted] = @IsDeleted" +
                              "INNER JOIN [MenuItems] ON [RecipieItems].[ItemId] = [MenuItems].[ItemId]" +
                              "WHERE [ItemId] = @ItemId ";

            DataAccessService.PersistObjectAsync(menuItem, UpdateQuery);
        }

        public Models.MenuItem GetByMenuItemId(System.Guid itemId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuItems].[ItemId], [MenuItems].[ItemVersionId], [MenuItems].[Name], [MenuItems].[Modified], " +
                                "[MenuItems].[ModifiedBy], [MenuItems].[MenuCategoryId], " +
                                "[RecipieItems].[ItemId], [RecipieItems].[ItemVersionId] FROM [MenuItems] " +
                                "INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId] " +
                                "WHERE [MenuItems].[ItemId] = @itemId AND [MenuItems].[IsDeleted] = @isDeleted";

            var result = DataAccessService.Get<Models.MenuItem>(SelectQuery, new { itemId, isDeleted });
            return result.FirstOrDefault();
        }

        public IEnumerable<Models.MenuItem> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [MenuItems].[ItemId], [MenuItems].[ItemVersionId], [MenuItems].[Name], [MenuItems].[Modified], " +
                                "[MenuItems].[ModifiedBy], [MenuItems].[MenuCategoryId], " +
                                "[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId] FROM [MenuItems] " +
                                "INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId]" +
                                "WHERE [MenuItems].[IsDeleted] = @isDeleted";

            var result = DataAccessService.Get<Models.MenuItem>(SelectQuery, new { isDeleted });
            return result.ToList();
        }

    }
}
