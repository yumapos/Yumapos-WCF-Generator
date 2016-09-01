﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
public partial class MenuItemVersionRepository : RepositoryBase
{
private const string InsertQuery = @"DECLARE @TempPKTable TABLE (ItemVersionId );
DECLARE @TempPKItemVersionId ;
INSERT INTO RecipieItems([ItemId],[ItemVersionId],[IsDeleted],[CategoryId])
OUTPUT INSERTED.ItemVersionId INTO @TempPKTable
VALUES (@ItemId,@ItemVersionId,@IsDeleted,@CategoryId)
SELECT @TempPKItemVersionId = ItemVersionId FROM @TempPKTable
INSERT INTO MenuItemVersionRepository([Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId])
VALUES (@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId)
SELECT ItemVersionId FROM @TempPKTable
";
public MenuItemVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var res = DataAccessService.InsertObject(menuItem, InsertQuery);
return (Guid)res;
}
public async Task<Guid> InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var res = await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
return (Guid)res;
}

}
}
