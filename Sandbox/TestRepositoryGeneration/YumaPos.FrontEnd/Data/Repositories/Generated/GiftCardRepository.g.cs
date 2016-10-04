﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumaPos.FrontEnd.Infrastructure.Repositories;


namespace YumaPos.FrontEnd.Data.Repositories.TransactionsHistory
{
	public partial class GiftCardRepository : RepositoryBase, IGiftCardRepository
	{
		private const string Fields = @"[dbo].[GiftCards].[GiftCardId],[dbo].[GiftCards].[Balance],[dbo].[GiftCards].[OpenDate],[dbo].[GiftCards].[ExpDate],[dbo].[GiftCards].[Active],[dbo].[GiftCards].[Modified]{columns}";
		private const string Values = @"@GiftCardId,@Balance,@OpenDate,@ExpDate,@Active,@Modified{values}";
		private const string SelectAllQuery = @"SELECT [dbo].[GiftCards].[GiftCardId],[dbo].[GiftCards].[Balance],[dbo].[GiftCards].[OpenDate],[dbo].[GiftCards].[ExpDate],[dbo].[GiftCards].[Active],[dbo].[GiftCards].[Modified] FROM [dbo].[GiftCards]  {whereTenantId:[dbo].[GiftCards]} ";
		private const string SelectByQuery = @"SELECT [dbo].[GiftCards].[GiftCardId],[dbo].[GiftCards].[Balance],[dbo].[GiftCards].[OpenDate],[dbo].[GiftCards].[ExpDate],[dbo].[GiftCards].[Active],[dbo].[GiftCards].[Modified] FROM [dbo].[GiftCards] ";
		private const string InsertQuery = @"INSERT INTO [dbo].[GiftCards]([dbo].[GiftCards].[GiftCardId],[dbo].[GiftCards].[Balance],[dbo].[GiftCards].[OpenDate],[dbo].[GiftCards].[ExpDate],[dbo].[GiftCards].[Active],[dbo].[GiftCards].[Modified]{columns}) OUTPUT INSERTED.GiftCardId VALUES(@GiftCardId,@Balance,@OpenDate,@ExpDate,@Active,@Modified{values}) ";
		private const string UpdateQueryBy = @"UPDATE [dbo].[GiftCards] SET [dbo].[GiftCards].[GiftCardId] = @GiftCardId,[dbo].[GiftCards].[Balance] = @Balance,[dbo].[GiftCards].[OpenDate] = @OpenDate,[dbo].[GiftCards].[ExpDate] = @ExpDate,[dbo].[GiftCards].[Active] = @Active,[dbo].[GiftCards].[Modified] = @Modified FROM [dbo].[GiftCards] ";
		private const string DeleteQueryBy = @"DELETE FROM [dbo].[GiftCards] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [dbo].[GiftCards].[GiftCardId] FROM [dbo].[GiftCards] ";
		private const string WhereQueryByGiftCardId = "WHERE [dbo].[GiftCards].[GiftCardId] = @GiftCardId{andTenantId:[dbo].[GiftCards]} ";


		/*
		public IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard> GetAll()
		{
		var sql = SelectAllQuery;
		var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, null).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>> GetAllAsync()
		{
		var sql = SelectAllQuery;
		var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, null));
		return result.ToList();
		}

		*/
		public YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard GetByGiftCardId(string giftCardId)
		{
			object parameters = new { giftCardId };
			var sql = SelectByQuery + WhereQueryByGiftCardId;
			var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard> GetByGiftCardIdAsync(string giftCardId)
		{
			object parameters = new { giftCardId };
			var sql = SelectByQuery + WhereQueryByGiftCardId;
			var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, parameters));
			return result.FirstOrDefault();
		}


		/*
		public string Insert(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
		var res = DataAccessService.InsertObject(giftCard,InsertQuery);
		return (string)res;
		}
		public async Task<string> InsertAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
		var res = await DataAccessService.InsertObjectAsync(giftCard,InsertQuery);
		return (string)res;
		}

		*/
		public void UpdateByGiftCardId(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
			var sql = UpdateQueryBy + WhereQueryByGiftCardId;
			DataAccessService.PersistObject(giftCard, sql);
		}
		public async Task UpdateByGiftCardIdAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
			var sql = UpdateQueryBy + WhereQueryByGiftCardId;
			await DataAccessService.PersistObjectAsync(giftCard, sql);
		}


		/*
		public void RemoveByGiftCardId(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
		var sql = DeleteQueryBy + WhereQueryByGiftCardId; 
		DataAccessService.PersistObject(giftCard, sql);
		}
		public async Task RemoveByGiftCardIdAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard giftCard)
		{
		var sql = DeleteQueryBy + WhereQueryByGiftCardId; 
		await DataAccessService.PersistObjectAsync(giftCard, sql);
		}

		public void RemoveByGiftCardId(string giftCardId)
		{
		object parameters = new {giftCardId};
		var sql = DeleteQueryBy + WhereQueryByGiftCardId; 
		DataAccessService.PersistObject<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, parameters);
		}
		public async Task RemoveByGiftCardIdAsync(string giftCardId)
		{
		object parameters = new {giftCardId};
		var sql = DeleteQueryBy + WhereQueryByGiftCardId; 
		await DataAccessService.PersistObjectAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.GiftCard>(sql, parameters);
		}


		*/

	}
}
