﻿using System.Collections.Generic;
using System.Text;
using WCFGenerator.SerializeGeneration.Helpers;
using WCFGenerator.SerializeGeneration.Models;

namespace WCFGenerator.SerializeGeneration.Generation
{
    public class TextSerializationPatterns
    {
        private GenerationElements _generationElements;
        public TextSerializationPatterns(GenerationElements generationElements)
        {
            _generationElements = generationElements;
        }

        private StringBuilder GenerateInterface(GenerationElements elements)
        {
            var exitInterface = new StringBuilder();
            exitInterface.AppendFormat("\t {1} partial class {0} : {2}IBoDo", elements.GeneratedClassName, elements.ClassAccessModificator,
                string.IsNullOrEmpty(elements.SerializableBaseClassName) ? "" : elements.SerializableBaseClassName + ",");
            exitInterface.Append("\r\n");
            exitInterface.Append("\t {");
            exitInterface.Append("\r\n");
            foreach (var member in elements.Properties)
            {
                exitInterface.Append("\t\t public ");
                exitInterface.Append(member.Type.Trim() == "PosMoney" ? "decimal?" : member.Type);
                exitInterface.Append(" ");
                exitInterface.Append(member.Name);
                exitInterface.Append(" { get; set;}");
                exitInterface.Append("\r\n");
            }

            exitInterface.Append("\t }");
            exitInterface.Append("\r\n");

            return exitInterface;
        }

        public StringBuilder GeneratePartialClass()
        {
            var generation = new StringBuilder();

            generation.Append(
                        "//------------------------------------------------------------------------------\r\n"
                        + "// <auto-generated>\r\n"
                        + "//     This code was generated from a template.\r\n"
                        + "//\r\n"
                        + "//     Manual changes to this file may cause unexpected behavior in your application.\r\n"
                        + "//     Manual changes to this file will be overwritten if the code is regenerated.\r\n"
                        + "// </auto-generated>\r\n"
                        + "//------------------------------------------------------------------------------\r\n\r\n");

            foreach (var namesp in _generationElements.UsingNamespaces)
            {
                generation.Append("using ");
                generation.Append(namesp);
                generation.Append("; \r\n");
            }
            generation.Append("\r\n");
            generation.Append("namespace ");
            generation.Append(_generationElements.Namespace);
            generation.Append("\r\n{");
            generation.Append("\r\n");

            var className = _generationElements.GeneratedClassName;
            if (_generationElements.IsPropertyEquals)
            {
                className = _generationElements.MapClassName;
            }
            else
            {
                generation.Append(GenerateInterface(_generationElements));
            }

            generation.AppendFormat("\t {0} partial class {1} {2}", _generationElements.ClassAccessModificator, _generationElements.ClassName,
                !string.IsNullOrEmpty(_generationElements.SerializableBaseClassName) ? "" : ": StatefulObject");
            generation.Append("\r\n");
            generation.Append("\t {");
            generation.Append("\r\n");

            generation.Append(GenerateMethodsOrMappings(_generationElements.Properties, className, true));
            if (_generationElements.MapClassName != null && _generationElements.MapProperties != null)
            {
                generation.Append("\r\n");
                generation.Append(GenerateMethodsOrMappings(_generationElements.MapProperties, _generationElements.MapClassName, false));
            }

            generation.Append("\r\n");
            generation.Append("\t }");
            generation.Append("\r\n");
            generation.Append("}");

            return generation;
        }

        private StringBuilder GenerateMethodsOrMappings(List<GenerationProperty> mainProp, string className, bool isSerialization)
        {
            var variableName = EditingSerializationHelper.FirstSymbolToLower(className);
            var stringBuilder = new StringBuilder();

            var functionPrefix = isSerialization ? "Do" : "BDo";
            var getMethodsName = isSerialization ? "GetDataObject" : $"Get{className}Do";
            var setMethodsName = isSerialization ? "SetDataObject" : $"Set{className}Do";

            //Generate virtual method for SetState
            stringBuilder.AppendFormat("\t\t partial void {2}CustomizationOnSet({0} {1} {3});", className, variableName, functionPrefix,
                isSerialization ? ", YumaPos.FrontEnd.Infrastructure.Persistence.IDeserializationContext context" : "");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\r\n");

            //Generate virtual method for GetState
            stringBuilder.AppendFormat("\t\t partial void {2}CustomizationOnGet(ref {0} {1});", className, variableName, functionPrefix);
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\r\n");

            //Generate Set state
            stringBuilder.AppendFormat("\t\t public{2} void {0}(IBoDo value{1})", setMethodsName,
                isSerialization ? ", YumaPos.FrontEnd.Infrastructure.Persistence.IDeserializationContext context" : "", isSerialization ? " override" : "");

            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t {");
            if (isSerialization)
            {
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t base.SetDataObject(value, context);");
            }
            stringBuilder.Append("\r\n");
            stringBuilder.AppendFormat("\t\t\t var dataObject = value as {0};", className);
            stringBuilder.Append("\r\n");

            foreach (var prop in mainProp)
            {
                if (prop.Type.Trim() == "PosMoney")
                {
                    stringBuilder.AppendFormat("\t\t\t {0} = dataObject.{1}.HasValue ? new PosMoney(context.MonetarySettings)", prop.VariableClassName, prop.Name);
                    stringBuilder.Append("\r\n");
                    stringBuilder.Append("\t\t\t {");
                    stringBuilder.Append("\r\n");
                    stringBuilder.AppendFormat("\t\t\t\t Value = dataObject.{0}.Value", prop.Name);
                    stringBuilder.Append("\r\n");
                    stringBuilder.Append("\t\t\t } : null;");
                    stringBuilder.Append("\r\n");
                }
                else
                {
                    stringBuilder.AppendFormat("\t\t\t {1} = dataObject.{0};", prop.Name, prop.VariableClassName);
                    stringBuilder.Append("\r\n");
                }
            }
            stringBuilder.AppendFormat("\t\t\t {0}CustomizationOnSet(dataObject{1});", functionPrefix,
                isSerialization ? ",context" : "");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t }");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\r\n");

            //Generate Get state
            stringBuilder.AppendFormat("\t\t public {1} {0}({2})", getMethodsName,
                isSerialization ? "override IBoDo" : "object", isSerialization ? "IBoDo childBoDo = null" : "");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t {");
            stringBuilder.Append("\r\n");
            if (isSerialization)
            {
                stringBuilder.AppendFormat("\t\t\t {0} bodo = ({0})(childBoDo ?? BoDoInstance);", className);
                stringBuilder.Append("\r\n");
                stringBuilder.AppendFormat("\t\t\t bodo = ({0}) base.GetDataObject(bodo);", className);
            }
            else
            {
                stringBuilder.AppendFormat("\t\t\t\t var dataObject = new {0}();", className);
            }

            stringBuilder.Append("\r\n");
            foreach (var prop in mainProp)
            {
                if (prop.Type.Trim() == "PosMoney")
                {
                    stringBuilder.AppendFormat("\t\t\t {2}.{0} = {1} != null ? {1}.Value : (decimal?) null;", prop.Name, prop.VariableClassName,
                        isSerialization ? "bodo" : "dataObject");
                    stringBuilder.Append("\r\n");
                }
                else
                {
                    stringBuilder.AppendFormat("\t\t\t {2}.{0} = {1};", prop.Name, prop.VariableClassName,
                        isSerialization ? "bodo" : "dataObject");
                    stringBuilder.Append("\r\n");
                }
            }
            stringBuilder.AppendFormat("\t\t\t {0}CustomizationOnGet(ref {1});", functionPrefix,
                isSerialization ? "bodo" : "dataObject");
            stringBuilder.Append("\r\n");
            if (isSerialization)
            {
                stringBuilder.Append("\t\t\t return bodo;");
            }
            else
            {
                stringBuilder.Append("\t\t\t return dataObject;");
            }

            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t }");

            if (isSerialization)
            {
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t protected override IBoDo BoDoInstance");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t {");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t get");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t {");
                stringBuilder.Append("\r\n");
                stringBuilder.AppendFormat("\t\t\t\t if (_BoDo == null) _BoDo = new {0}();", className);
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t\t return _BoDo;");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t }");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t set");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t {");
                stringBuilder.Append("\r\n");
                stringBuilder.AppendFormat("\t\t\t\t this._BoDo = value as {0};", className);
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t }");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t }");
                stringBuilder.Append("\r\n");
            }

            return stringBuilder;
        }
    }
}
