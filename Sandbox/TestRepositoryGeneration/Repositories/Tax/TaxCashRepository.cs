using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration.Repositories.Tax
{
    class TaxCashRepository : RepositoryBase
    {
        public TaxCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(Models.Tax tax)
        {
            var InsertQuery = "INSERT INTO [dbo].[Taxes] ([TaxId],[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted]) VALUES " +
                              "(@TaxId, @TaxVersionId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            DataAccessService.InsertObject(tax, InsertQuery);
        }

        public void Update(Models.Tax tax)
        {
            var InsertQuery = "UPDATE[dbo].[Taxes] SET [TaxVersionId] = @TaxVersionId, [Name] = @Name, [Modified] = @Modified," +
                            "[ModifiedBy] = @ModifiedBy, [IsDeleted] = @IsDeleted WHERE [TaxId] = @TaxId;";

            DataAccessService.InsertObject(tax, InsertQuery);
        }

        public Models.Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [TaxId] ,[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[Taxes] Where [TaxId] = @taxId";

            var result = DataAccessService.Get<Models.Tax>(SelectQuery, new { taxId });
            return result.FirstOrDefault();
        }

        public IEnumerable<Models.Tax> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [TaxId] ,[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[Taxes]";

            var result = DataAccessService.Get<Models.Tax>(SelectQuery, null);
            return result.ToList();
        }

    }
}
