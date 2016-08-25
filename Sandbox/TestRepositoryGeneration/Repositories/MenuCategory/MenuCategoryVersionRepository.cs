using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuCategoryVersionRepository : RepositoryBase
    {
        public MenuCategoryVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public Guid Insert(Models.MenuCategory menuCategory)
        {
            var InsertQuery = "INSERT INTO [History].[MenuCategoriesVersions] ([MenuCategoryId],[Name],[Modified],[ModifiedBy],[IsDeleted]) " +
                              "OUTPUT INSERTED.MenuCategoryVersionId " +
                              "VALUES (@MenuCategoryId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            return (Guid)DataAccessService.InsertObject(menuCategory, InsertQuery);
        }

    }
}
