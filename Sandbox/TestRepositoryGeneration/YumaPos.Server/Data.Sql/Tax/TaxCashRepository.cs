using System.Collections.Generic;
using System.Linq;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
using YumaPos.Server.Data.Sql.Menu;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Data.Sql.Taxes
{
    partial class TaxCashRepository : RepositoryBase
    {
        public TaxCashRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(Tax tax)
        {
            var InsertQuery = "INSERT INTO [dbo].[Taxes] ([TaxId],[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted]) VALUES " +
                              "(@TaxId, @TaxVersionId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            DataAccessService.InsertObject(tax, InsertQuery);
        }

        public void Update(Tax tax)
        {
            var InsertQuery = "UPDATE[dbo].[Taxes] SET [TaxVersionId] = @TaxVersionId, [Name] = @Name, [Modified] = @Modified," +
                            "[ModifiedBy] = @ModifiedBy, [IsDeleted] = @IsDeleted WHERE [TaxId] = @TaxId;";

            DataAccessService.InsertObject(tax, InsertQuery);
        }

        public Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [TaxId] ,[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[Taxes] Where [TaxId] = @taxId";

            var result = DataAccessService.Get<Tax>(SelectQuery, new { taxId });
            return result.FirstOrDefault();
        }

        public IEnumerable<Tax> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [TaxId] ,[TaxVersionId],[Name],[Modified],[ModifiedBy],[IsDeleted] FROM[dbo].[Taxes]";

            var result = DataAccessService.Get<Tax>(SelectQuery, null);
            return result.ToList();
        }

    }
}
