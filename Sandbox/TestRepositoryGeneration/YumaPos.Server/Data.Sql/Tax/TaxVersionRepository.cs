using System;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;

namespace YumaPos.Server.Data.Sql.Taxes
{
    class TaxVersionRepository : RepositoryBase
    {
        public TaxVersionRepository(IDataAccessService dataAccessService) : base(dataAccessService) { }

        public Guid Insert(Tax tax)
        {
            var InsertQuery = "INSERT INTO [History].[TaxesVersions] ([TaxId],[Name],[Modified],[ModifiedBy],[IsDeleted]) " +
                              "OUTPUT INSERTED.TaxVersionId " +
                              "VALUES (@TaxId, @Name, @Modified, @ModifiedBy, @IsDeleted)";

            return (Guid)DataAccessService.InsertObject(tax, InsertQuery);
        }
    }
}
