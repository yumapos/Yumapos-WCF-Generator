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

namespace TestWcfClientGenerator
{
	 public static class TaskExt
	 {
		 public static SimpleAwaiter<TResult> ContinueOnScope<TResult>(this Task<TResult> @this, FlowOperationContextScope scope) 
		 {
			 return new SimpleAwaiter<TResult>(@this, scope.BeforeAwait, scope.AfterAwait);
		 }

		 public static SimpleAwaiter ContinueOnScope(this Task @this, FlowOperationContextScope scope) 
		 {
			 return new SimpleAwaiter(@this, scope.BeforeAwait, scope.AfterAwait);
		 }
	 }
}