﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace TestDecoratorGeneration
{
	public partial class ManagerDecorator
	{

		private TestDecoratorGeneration.Manager DecoratedComponent { get; set; }

		public async System.Threading.Tasks.Task Start()
		{
			await OnEntryAsync("Start", new object[] { });
			await DecoratedComponent.Start();
		}
		public async System.Threading.Tasks.Task Stop()
		{
			await OnEntryAsync("Stop", new object[] { });
			await DecoratedComponent.Stop();
		}

	}
}
