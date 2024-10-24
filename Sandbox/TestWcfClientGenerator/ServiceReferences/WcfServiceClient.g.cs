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
	 public partial class WcfServiceClient : System.ServiceModel.ClientBase<IWcfServiceClient>, IWcfServiceClient, IProperter, IClientBase
 	 { 
		 private static string _remoteAddress = ""; 
 
		 public bool IsCaughtException { get; set; } 

		 public WcfServiceClient() : base(WcfServiceClient.GetDefaultBinding(), WcfServiceClient.GetDefaultEndpointAddress()) { } 

		 public WcfServiceClient(EndpointConfiguration endpointConfiguration) : base(WcfServiceClient.GetBindingForEndpoint(endpointConfiguration), WcfServiceClient.GetEndpointAddress(endpointConfiguration)) {   } 

		 public WcfServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(WcfServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) { 
 			 _remoteAddress = remoteAddress; 
 		 } 

		 public WcfServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : base(WcfServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)  { 
 			 _remoteAddress =  remoteAddress.Uri.AbsoluteUri; 
 		 } 

		 public WcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) {
 			 _remoteAddress =  remoteAddress.Uri.AbsoluteUri; 
 		  } 


		 private BeginOperationDelegate onBeginAddItemDelegate; 
		 private EndOperationDelegate onEndAddItemDelegate; 
		 private System.Threading.SendOrPostCallback onAddItemCompletedDelegate; 

		 public event System.EventHandler<WcfService_AddItemCompletedEventArgs> AddItemCompleted; 

		 private BeginOperationDelegate onBeginGetItemsDelegate; 
		 private EndOperationDelegate onEndGetItemsDelegate; 
		 private System.Threading.SendOrPostCallback onGetItemsCompletedDelegate; 

		 public event System.EventHandler<WcfService_GetItemsCompletedEventArgs> GetItemsCompleted; 

		 [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		 public System.IAsyncResult BeginAddItem(Guid id, string name, System.AsyncCallback callback, object asyncState)
		 {
		   return base.Channel.BeginAddItem(id, name,  callback, asyncState); 
		 }

		 [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		 public TestDecoratorGeneration.ResponseDto EndAddItem(System.IAsyncResult result)
		 {
		   try
		   {
		       return base.Channel.EndAddItem(result);
		   }
		   catch (Exception)
		   {
		       IsCaughtException = true;
		       throw;
		   }
		 }

		 private System.IAsyncResult OnBeginAddItem(object[] inValues, System.AsyncCallback callback, object asyncState)
		 {
		   Guid id = ((Guid)(inValues[0]));
		   string name = ((string)(inValues[1]));
		   return ((IWcfServiceClient)(this)).BeginAddItem(id, name, callback, asyncState);
		 }

		 private object[] OnEndAddItem(System.IAsyncResult result)
		 {
		   TestDecoratorGeneration.ResponseDto retVal = ((IWcfServiceClient)(this)).EndAddItem(result);
		   return new object[] { retVal };
		 }

		 private void OnAddItemCompleted(object state)
		 {
		   if ((this.AddItemCompleted != null))
		   {
		      InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
		      this.AddItemCompleted(this, new WcfService_AddItemCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		   }
		 }

		 public void AddItemAsync(Guid id, string name)
		 {
		   this.AddItemAsync(id, name, null);
		 }

		 public void AddItemAsync(Guid id, string name,  object userState)
		 {
		   if ((this.onBeginAddItemDelegate == null))
		   {
		       this.onBeginAddItemDelegate = new BeginOperationDelegate(this.OnBeginAddItem);
		   }
		   if ((this.onEndAddItemDelegate == null))
		   {
		       this.onEndAddItemDelegate = new EndOperationDelegate(this.OnEndAddItem);
		   }
		   if ((this.onAddItemCompletedDelegate == null))
		   {
		       this.onAddItemCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddItemCompleted);
		   }
		   base.InvokeAsync(this.onBeginAddItemDelegate, new object[] {id, name}, this.onEndAddItemDelegate, this.onAddItemCompletedDelegate, userState);
		       this.onAddItemCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddItemCompleted);
		 }

		 [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		 public System.IAsyncResult BeginGetItems( System.AsyncCallback callback, object asyncState)
		 {
		   return base.Channel.BeginGetItems( callback, asyncState); 
		 }

		 [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		 public TestDecoratorGeneration.ItemDto EndGetItems(System.IAsyncResult result)
		 {
		   try
		   {
		       return base.Channel.EndGetItems(result);
		   }
		   catch (Exception)
		   {
		       IsCaughtException = true;
		       throw;
		   }
		 }

		 private System.IAsyncResult OnBeginGetItems(object[] inValues, System.AsyncCallback callback, object asyncState)
		 {
		   return ((IWcfServiceClient)(this)).BeginGetItems(callback, asyncState);
		 }

		 private object[] OnEndGetItems(System.IAsyncResult result)
		 {
		   TestDecoratorGeneration.ItemDto retVal = ((IWcfServiceClient)(this)).EndGetItems(result);
		   return new object[] { retVal };
		 }

		 private void OnGetItemsCompleted(object state)
		 {
		   if ((this.GetItemsCompleted != null))
		   {
		      InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
		      this.GetItemsCompleted(this, new WcfService_GetItemsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
		   }
		 }

		 public void GetItemsAsync()
		 {
		   this.GetItemsAsync(null);
		 }

		 public void GetItemsAsync( object userState)
		 {
		   if ((this.onBeginGetItemsDelegate == null))
		   {
		       this.onBeginGetItemsDelegate = new BeginOperationDelegate(this.OnBeginGetItems);
		   }
		   if ((this.onEndGetItemsDelegate == null))
		   {
		       this.onEndGetItemsDelegate = new EndOperationDelegate(this.OnEndGetItems);
		   }
		   if ((this.onGetItemsCompletedDelegate == null))
		   {
		       this.onGetItemsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetItemsCompleted);
		   }
		   base.InvokeAsync(this.onBeginGetItemsDelegate, new object[] {}, this.onEndGetItemsDelegate, this.onGetItemsCompletedDelegate, userState);
		       this.onGetItemsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetItemsCompleted);
		 }

		 protected override IWcfServiceClient CreateChannel() 
 		 {
			 return new WcfServiceClientChannel(this); 
 		 } 

		 private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) 
 		 { 
			 if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) 
 			 { 
				 System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding(); 
				 result.MaxBufferSize = int.MaxValue; 
				 result.MaxReceivedMessageSize = int.MaxValue; 
				 return result; 
 			 } 
 			 throw new System.InvalidOperationException(string.Format("Could not find endpoint with name '{0}'.", endpointConfiguration));
 		 } 

		 private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) 
 		 { 
			 if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_Client)) 
 			 { 
				 return new System.ServiceModel.EndpointAddress(_remoteAddress); 
 			 } 

			 throw new System.InvalidOperationException(string.Format("Could not find endpoint with name '{0}'.", endpointConfiguration));
 		 } 

		 private static System.ServiceModel.Channels.Binding GetDefaultBinding() 
 		 { 
			 return WcfServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_Client); 
 		 } 
		 private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() 
 		 { 
			 return WcfServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_Client); 
 		 } 

		 private class WcfServiceClientChannel : ChannelBase<IWcfServiceClient>, IWcfServiceClient, IProperter 
 		 { 
			 public bool IsCaughtException { get; set; } 

		 public WcfServiceClientChannel(System.ServiceModel.ClientBase<IWcfServiceClient> client) : base(client) { } 

		 public System.IAsyncResult BeginAddItem(Guid id, string name, System.AsyncCallback callback, object asyncState) 
 		 { 
			 object[] _args = new object[2]; 
			 _args[0] = id;
			 _args[1] = name;
			 System.IAsyncResult _result = base.BeginInvoke("AddItem", _args, callback, asyncState);
			 return _result;
		 } 

		 public TestDecoratorGeneration.ResponseDto EndAddItem(System.IAsyncResult result) 
 		 { 
			 object[] _args = new object[0];
			 TestDecoratorGeneration.ResponseDto _result = ((TestDecoratorGeneration.ResponseDto)(base.EndInvoke("AddItem", _args, result)));
			 return _result;
		 } 

		 public System.IAsyncResult BeginGetItems(System.AsyncCallback callback, object asyncState) 
 		 { 
			 object[] _args = new object[0]; 
			 System.IAsyncResult _result = base.BeginInvoke("GetItems", _args, callback, asyncState);
			 return _result;
		 } 

		 public TestDecoratorGeneration.ItemDto EndGetItems(System.IAsyncResult result) 
 		 { 
			 object[] _args = new object[0];
			 TestDecoratorGeneration.ItemDto _result = ((TestDecoratorGeneration.ItemDto)(base.EndInvoke("GetItems", _args, result)));
			 return _result;
		 } 

	 } 
    } 
}