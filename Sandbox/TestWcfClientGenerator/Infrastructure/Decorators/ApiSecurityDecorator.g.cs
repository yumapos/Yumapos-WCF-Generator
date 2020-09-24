﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using YumaPos.Shared.Terminal.Infrastructure;
using YumaPos.FrontEnd.Infrastructure.CommandProcessing;

namespace TestWcfClientGenerator
{
	public sealed class ApiSecurityDecorator : TestWcfClientGenerator.IWcfServiceApi
	{
		private readonly TestWcfClientGenerator.IWcfServiceApi _actor;
		#region Properties
		public ExecutionContext ExecutionContext
		{
			get { return _actor.ExecutionContext; }
			set { _actor.ExecutionContext = value; }
		}
		#endregion
		public ApiSecurityDecorator(TestWcfClientGenerator.IWcfServiceApi actor)
		{
			if (actor == null) throw new ArgumentNullException(nameof(actor));
			_actor = actor;
		}
		public async System.Threading.Tasks.Task<TestDecoratorGeneration.ResponseDto> AddItem(System.Guid id, string name)
		{
			var response = await _actor.AddItem(id, name);
			if (!response.Context.IsEmpty() || response.PostprocessingType != null)
			{
				throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo) { Value = response.Value };
			}
			return response;
		}

		public async System.Threading.Tasks.Task<TestDecoratorGeneration.ItemDto> GetItems()
		{
			var response = await _actor.GetItems();
			if (!response.Context.IsEmpty() || response.PostprocessingType != null)
			{
				throw new ServerSecurityException(response.Context, response.PostprocessingType, response.Errors, response.ServerInfo) { Value = response.Value };
			}
			return response;
		}

	}
}
