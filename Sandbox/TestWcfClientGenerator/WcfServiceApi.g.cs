﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System; 
using System.Collections.Generic; 
using System.ServiceModel; 
using System.Threading.Tasks; 

 namespace TestWcfClientGenerator
 { 
	 public partial class WcfServiceApi
	 { 
		 private ChannelContainer<T> CreateChannel<T>() where T : class, IProperter, new()
		 { 
			 var clientContainer = ClientFactory<T>.CreateClient(_endPoint.ToString(), _binding, _endPoint); 
			 return clientContainer; 
		 } 

		 public async System.Threading.Tasks.Task<TestDecoratorGeneration.ResponseDto> AddItem(System.Guid id, string name)
		 {
			 var channelContainer = CreateChannel<WcfServiceClient> ();
			 var scope = new FlowOperationContextScope(channelContainer.Client.InnerChannel);

			 try
			 {
				 AddClientInformationHeader();
				 return await System.Threading.Tasks.Task<TestDecoratorGeneration.ResponseDto>.Factory.FromAsync<Guid, string>(channelContainer.Client.BeginAddItem, channelContainer.Client.EndAddItem, id, name,  null).ContinueOnScope(scope);
			 }
			 finally
			 {
				 var disposable = channelContainer as IDisposable; 
				 if (disposable != null) disposable.Dispose();

				 disposable = scope as IDisposable;
				 if (disposable != null) disposable.Dispose();
			 }
		 }

		 public async System.Threading.Tasks.Task<TestDecoratorGeneration.ItemDto[]> GetItems()
		 {
			 var channelContainer = CreateChannel<WcfServiceClient> ();
			 var scope = new FlowOperationContextScope(channelContainer.Client.InnerChannel);

			 try
			 {
				 AddClientInformationHeader();
				 return await System.Threading.Tasks.Task<TestDecoratorGeneration.ItemDto[]>.Factory.FromAsync(channelContainer.Client.BeginGetItems, channelContainer.Client.EndGetItems,  null).ContinueOnScope(scope);
			 }
			 finally
			 {
				 var disposable = channelContainer as IDisposable; 
				 if (disposable != null) disposable.Dispose();

				 disposable = scope as IDisposable;
				 if (disposable != null) disposable.Dispose();
			 }
		 }

		 public async System.Threading.Tasks.Task Open()
		 {
			 var channelContainer = CreateChannel<WcfServiceClient> ();
			 var scope = new FlowOperationContextScope(channelContainer.Client.InnerChannel);

			 try
			 {
				 AddClientInformationHeader();
				 await System.Threading.Tasks.Task.Factory.FromAsync(channelContainer.Client.BeginOpen, channelContainer.Client.EndOpen,  null).ContinueOnScope(scope);
			 }
			 finally
			 {
				 var disposable = channelContainer as IDisposable; 
				 if (disposable != null) disposable.Dispose();

				 disposable = scope as IDisposable;
				 if (disposable != null) disposable.Dispose();
			 }
		 }

	 } }