using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuItemVersionRepository : RepositoryBase
    {
        public MenuItemVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public Guid Insert(Models.MenuItem menuItem)
        {
            var InsertQuery = "DECLARE @TempPKTable TABLE (ItemVersionId uniqueidentifier); " +
                              "DECLARE @TempPKItemVersionId uniqueidentifier; " +
                              "INSERT INTO [History].[RecipieItemsVersions] ([ItemId], [IsDeleted]) " +
                              "OUTPUT INSERTED.ItemVersionId INTO @TempPKTable " +
                              "VALUES (@ItemId, @IsDeleted)" +
                              "SELECT @TempPKItemVersionId = ItemVersionId FROM @TempPKTable " +
                              "INSERT INTO [History].[MenuItemsVersions] ([ItemId], [ItemVersionId], [Name], [Modified], [ModifiedBy], [MenuCategoryId], [IsDeleted]) " +
                              "VALUES (@ItemId, @TempPKItemVersionId, @Name, @Modified, @ModifiedBy, @MenuCategoryId, @IsDeleted) " +
                              "SELECT ItemVersionId FROM @TempPKTable";

            return (Guid)DataAccessService.InsertObject(menuItem, InsertQuery);
        }
    }
}
