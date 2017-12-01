﻿//The file Extensions/TestClass.g.cs was automatically generated using WCF-Generator.exe


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic; 

namespace TestSerializationGeneration
{
	 public partial class TestClassDo : IBoDo
	 {
		 public string Property { get; set;}
		 public Dictionary<string,string> PropertyWithInvalidType { get; set;}
		 public string SerializeIncludeProperty { get; set;}
	 }
	 public partial class TestClass : StatefulObject
	 {
		 partial void DoCustomizationOnSet(TestClassDo testClassDo , YumaPos.FrontEnd.Infrastructure.Persistence.IDeserializationContext context);

		 partial void DoCustomizationOnGet(ref TestClassDo testClassDo);

		 public override void SetDataObject(IBoDo value, YumaPos.FrontEnd.Infrastructure.Persistence.IDeserializationContext context)
		 {
			 base.SetDataObject(value, context);
			 var dataObject = value as TestClassDo;
			 Property = dataObject.Property;
			 SerializeIncludeProperty = dataObject.SerializeIncludeProperty;
			 DoCustomizationOnSet(dataObject,context);
		 }

		 public override IBoDo GetDataObject(IBoDo childBoDo = null)
		 {
			 TestClassDo bodo = (TestClassDo)(childBoDo ?? BoDoInstance);
			 bodo = (TestClassDo) base.GetDataObject(bodo);
			 bodo.Property = Property;
			 bodo.SerializeIncludeProperty = SerializeIncludeProperty;
			 DoCustomizationOnGet(ref bodo);
			 return bodo;
		 }
		 protected override IBoDo BoDoInstance
		 {
			 get
			 {
				 if (_BoDo == null) _BoDo = new TestClassDo();
				 return _BoDo;
			 }
			 set
			 {
				 this._BoDo = value as TestClassDo;
			 }
		 }

	 }
}