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

		public System.Threading.Tasks.Task Start()
		{
			DecoratedComponent.Start();
		}
		public System.Threading.Tasks.Task Stop()
		{
			DecoratedComponent.Stop();
		}

	}
}
