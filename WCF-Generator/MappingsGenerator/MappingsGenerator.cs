﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using WCFGenerator.Common;

namespace WCFGenerator.MappingsGenerator
{
    public class MapDtoAndDo
    {
        public ClassDeclarationSyntax DOClass { get; set; }
        public ClassDeclarationSyntax DtoClass { get; set; }
        public InterfaceDeclarationSyntax DOInterface { get; set; }
        public InterfaceDeclarationSyntax DtoInterface { get; set; }
        public IEnumerable<MapPropertiesDtoAndDo> MapPropertiesDtoAndDo { get; set; }
    }

    public class MapPropertiesDtoAndDo
    {
        public PropertyDeclarationSyntax DOPropertyName { get; set; }
        public PropertyDeclarationSyntax DTOPropertyName { get; set; }
        public TypeOfExpressionSyntax DOPropertyType { get; set; }
        public TypeOfExpressionSyntax DtoropertyType { get; set; }
        public string ToDtoFunction { get; set; }
        public string FromDtoFunction { get; set; }
        public KindOfProperty KindOMapfProperty { get; set; }
    }

    public enum KindOfProperty
    {
        AttributeClass = 1,
        CollectionAttributeClasses = 2,
        FunctionAttribute = 3,
        None = 9
    }

    public class MappingsGenerator
    {
        private static readonly MSBuildWorkspace _workspace = MSBuildWorkspace.Create();

        private static Solution _solution = null;
        private static Project _project = null;

        public string MapExtensionClassName { get; set; }
        public string MapExtensionNameSpace { get; set; }

        public List<string> DoProjects { get; set; }
        public List<string> DtoProjects { get; set; }

        public string SolutionPath { get; set; }

        public MappingsGenerator()
        {
            DoProjects = new List<string>();
            DtoProjects = new List<string>();
        }

        public string MapAttribute { get; set; }
        public string MapIgnoreAttribute { get; set; }

        public bool DOSkipAttribute { get; set; }
        public bool DTOSkipAttribute { get; set; }

        public string DoSuffix { get; set; }
        public string DtoSuffix { get; set; }

        public Dictionary<string, string[]> SystemNullableTypes = new Dictionary<string, string[]>
        {
            {
                "int",
                new string[] {"int?", "System.Int32?", "System.Nullable<int>", "System.Nullable<System.Int32>"}
            },
            {
                "System.Int32",
                new string[] {"int?", "System.Int32?", "System.Nullable<int>", "System.Nullable<System.Int32>"}
            },

            {
                "short",
                new string[] {"short?", "System.Int16?", "System.Nullable<short>", "System.Nullable<System.Int16>"}
            },
            {
                "System.Int16",
                new string[] {"short?", "System.Int16?", "System.Nullable<short>", "System.Nullable<System.Int16>"}
            },

            {
                "long",
                new string[] {"long?", "System.Int64?", "System.Nullable<long>", "System.Nullable<System.Int64>"}
            },
            {
                "System.Int64",
                new string[] {"long?", "System.Int64?", "System.Nullable<long>", "System.Nullable<System.Int64>"}
            },

            {
                "decimal",
                new string[]
                {"decimal?", "System.Decimal?", "System.Nullable<decimal>", "System.Nullable<System.Decimal>"}
            },
            {
                "System.Decimal",
                new string[]
                {"decimal?", "System.Decimal?", "System.Nullable<decimal>", "System.Nullable<System.Decimal>"}
            },

            {
                "float",
                new string[] {"float?", "System.Single?", "System.Nullable<float>", "System.Nullable<System.Single>"}
            },
            {
                "System.Single",
                new string[] {"float?", "System.Single?", "System.Nullable<float>", "System.Nullable<System.Single>"}
            },

            {
                "double",
                new string[] {"double?", "System.Double?", "System.Nullable<double>", "System.Nullable<System.Double>"}
            },
            {
                "System.Double",
                new string[] {"double?", "System.Double?", "System.Nullable<double>", "System.Nullable<System.Double>"}
            },

            {
                "bool",
                new string[] {"bool?", "System.Boolean?", "System.Nullable<bool>", "System.Nullable<System.Boolean>"}
            },
            {
                "System.Boolean",
                new string[] {"bool?", "System.Boolean?", "System.Nullable<bool>", "System.Nullable<System.Boolean>"}
            },

            {
                "byte",
                new string[] {"byte?", "System.Byte?", "System.Nullable<byte>", "System.Nullable<System.Byte>"}
            },
            {
                "System.Byte",
                new string[] {"byte?", "System.Byte?", "System.Nullable<byte>", "System.Nullable<System.Byte>"}
            },

            {
                "Guid",
                new string[] {"Guid?", "System.Guid?", "System.Nullable<Guid>", "System.Nullable<System.Guid>"}
            },
            {
                "System.Guid",
                new string[] {"Guid?", "System.Guid?", "System.Nullable<Guid>", "System.Nullable<System.Guid>"}
            },

            {
                "char",
                new string[] {"char?", "System.Char?", "System.Nullable<char>", "System.Nullable<System.Char>"}
            },
            {
                "System.Char",
                new string[] {"char?", "System.Char?", "System.Nullable<char>", "System.Nullable<System.Char>"}
            },

            {
                "DateTime",
                new string[]
                {"DateTime?", "System.DateTime?", "System.Nullable<DateTime>", "System.Nullable<System.DateTime>"}
            },
            {
                "System.DateTime",
                new string[]
                {"DateTime?", "System.DateTime?", "System.Nullable<DateTime>", "System.Nullable<System.DateTime>"}
            },

            {
                "sbyte",
                new string[] {"sbyte?", "System.SByte?", "System.Nullable<sbyte>", "System.Nullable<System.SByte>"}
            },
            {
                "System.SByte",
                new string[] {"sbyte?", "System.SByte?", "System.Nullable<sbyte>", "System.Nullable<System.SByte>"}
            },

            {
                "uint",
                new string[] {"uint?", "System.UInt32?", "System.Nullable<uint>", "System.Nullable<System.UInt32>"}
            },
            {
                "System.UInt32",
                new string[] {"uint?", "System.UInt32?", "System.Nullable<uint>", "System.Nullable<System.UInt32>"}
            },

            {
                "ulong",
                new string[] {"ulong?", "System.UInt64?", "System.Nullable<ulong>", "System.Nullable<System.UInt64>"}
            },
            {
                "System.UInt64",
                new string[] {"ulong?", "System.UInt64?", "System.Nullable<ulong>", "System.Nullable<System.UInt64>"}
            },

            {
                "ushort",
                new string[] {"ushort?", "System.UInt16?", "System.Nullable<ushort>", "System.Nullable<System.UInt16>"}
            },
            {
                "System.UInt16",
                new string[] {"ushort?", "System.UInt16?", "System.Nullable<ushort>", "System.Nullable<System.UInt16>"}
            },
        };

        public bool IsInNullableDictionary(string systemtype, string nullableType)
        {
            var nullableCollectionValue =
                SystemNullableTypes.FirstOrDefault(x => x.Key.ToString().Trim() == systemtype).Value;

            var nullableCollection = nullableCollectionValue?.ToList();
            if (nullableCollection?.FirstOrDefault(x => x == nullableType) != null)
                return true;

            return false;
        }

        //public string GetMapName(ClassDeclarationSyntax element)
        //{
        //    var value = element.Identifier;
        //    const string nameProperty = "Name";

        //    Attributes attributes = null;

        //    var codeClass = element as CodeClass;
        //    if (codeClass != null)
        //    {
        //        attributes = codeClass.Attributes;
        //    }

        //    var codeProperty = element as CodeProperty;
        //    if (codeProperty != null)
        //    {
        //        attributes = codeProperty.Attributes;
        //    }

        //    foreach (CodeAttribute ca in attributes)
        //    {
        //        if (ca.Name.Contains(MapAttribute) && ca.Value.Contains(nameProperty))
        //        {
        //            value = ca.Value.Remove(0, ca.Value.IndexOf(nameProperty));
        //            value = value.Replace(" ", "");

        //            if (value.Contains(","))
        //            {
        //                value = value.Remove(value.IndexOf(","));
        //            }

        //            value = value.Remove(0, nameProperty.Length + 1);
        //            value = value.Replace("\"", "").ToLower();

        //            if (DoSuffix != null && codeClass != null)
        //            {
        //                if (value.EndsWith(DoSuffix.ToLower()))
        //                {
        //                    value = value.Replace(DoSuffix.ToLower(), "");
        //                }
        //            }

        //            if (DtoSuffix != null && codeClass != null)
        //            {
        //                if (value.EndsWith(DtoSuffix.ToLower()))
        //                {
        //                    value = value.Replace(DtoSuffix.ToLower(), "");
        //                }
        //            }
        //        }
        //    }

        //    value = value.ToLower();

        //    if (DoSuffix != null && codeClass != null)
        //    {
        //        if (value.EndsWith(DoSuffix.ToLower()))
        //        {
        //            value = value.Replace(DoSuffix.ToLower(), "");
        //        }
        //    }

        //    if (DtoSuffix != null && codeClass != null)
        //    {
        //        if (value.EndsWith(DtoSuffix.ToLower()))
        //        {
        //            value = value.Replace(DtoSuffix.ToLower(), "");
        //        }
        //    }

        //    return value;
        //}

        //public bool CompareTwoPropertyType(CodeProperty DOCodeProperty, CodeProperty DtoCodeProperty,
        //    List<MapDtoAndDo> similarClasses)
        //{
        //    var DOType = DOCodeProperty.Type.AsString;
        //    var DtoType = DtoCodeProperty.Type.AsString;

        //    if (DOType.Contains("IEnumerable") && DtoType.Contains("IEnumerable"))
        //    {
        //        DOType = DOType.Remove(0, DOType.IndexOf("<") + 1);
        //        DOType = DOType.Replace(">", "");

        //        DtoType = DtoType.Remove(0, DtoType.IndexOf("<") + 1);
        //        DtoType = DtoType.Replace(">", "");

        //        if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
        //            return true;
        //    }

        //    if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
        //        return true;

        //    if (DOType == DtoType)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //public bool CompareTwoPropertyType(string DOType, string DtoType, List<MapDtoAndDo> similarClasses)
        //{
        //    if (DOType.Contains("IEnumerable") && DtoType.Contains("IEnumerable"))
        //    {
        //        DOType = DOType.Remove(0, DOType.IndexOf("<") + 1);
        //        DOType = DOType.Replace(">", "");

        //        DtoType = DtoType.Remove(0, DtoType.IndexOf("<") + 1);
        //        DtoType = DtoType.Replace(">", "");

        //        if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
        //            return true;
        //    }

        //    if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
        //        return true;

        //    if (DOType == DtoType)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //public bool CompareInMapDtoAndDoCollection(string DOType, string DtoType, List<MapDtoAndDo> similarClasses)
        //{
        //    var similarInterfacesDO =
        //        similarClasses.Where(x => x.DOInterface != null).FirstOrDefault(x => x.DOInterface.FullName == DOType);
        //    var similarInterfacesDto =
        //        similarClasses.Where(x => x.DtoInterface != null)
        //            .FirstOrDefault(x => x.DtoInterface.FullName == DtoType);
        //    var checkingMapClass = new MapDtoAndDo();

        //    checkingMapClass = similarClasses.FirstOrDefault(x => x.DOClass.FullName == DOType);
        //    if (checkingMapClass != null &&
        //        checkingMapClass == similarClasses.FirstOrDefault(x => x.DtoClass.FullName == DtoType))
        //    {
        //        return true;
        //    }

        //    similarInterfacesDO =
        //        similarClasses.Where(x => x.DOInterface != null).FirstOrDefault(x => x.DOInterface.FullName == DOType);
        //    similarInterfacesDto =
        //        similarClasses.Where(x => x.DtoInterface != null)
        //            .FirstOrDefault(x => x.DtoInterface.FullName == DtoType);

        //    if (similarInterfacesDO != null &&
        //        similarInterfacesDO == similarClasses.FirstOrDefault(x => x.DtoClass.FullName == DtoType))
        //    {
        //        return true;
        //    }

        //    if (similarInterfacesDto != null &&
        //        similarClasses.FirstOrDefault(x => x.DOClass.FullName == DOType) == similarInterfacesDto)
        //    {
        //        return true;
        //    }

        //    if (similarInterfacesDO != null && similarInterfacesDO == similarInterfacesDto)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //public KindOfProperty GetKindOfMapProperty(CodeProperty codeProperty, List<MapDtoAndDo> listOfSimilarClasses)
        //{
        //    var type = codeProperty.Type.AsString;

        //    if (type.Contains("IEnumerable"))
        //    {
        //        type = type.Remove(0, type.IndexOf("<") + 1);
        //        type = type.Replace(">", "");

        //        if (listOfSimilarClasses.Any(x => x.DOClass.FullName == type) ||
        //            listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.FullName == type))
        //        {
        //            return KindOfProperty.CollectionAttributeClasses;
        //        }

        //        if (listOfSimilarClasses.Any(x => x.DtoClass.FullName == type) ||
        //            listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.FullName == type))
        //        {
        //            return KindOfProperty.CollectionAttributeClasses;
        //        }
        //    }

        //    if (listOfSimilarClasses.Any(x => x.DOClass.FullName == type) ||
        //        listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.FullName == type))
        //    {
        //        return KindOfProperty.AttributeClass;
        //    }

        //    if (listOfSimilarClasses.Any(x => x.DtoClass.FullName == type) ||
        //        listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.FullName == type))
        //    {
        //        return KindOfProperty.AttributeClass;
        //    }

        //    return KindOfProperty.None;
        //}

        //public KindOfProperty GetKindOfMapProperty(string type, List<MapDtoAndDo> listOfSimilarClasses)
        //{
        //    if (type.Contains("IEnumerable"))
        //    {
        //        type = type.Remove(0, type.IndexOf("<") + 1);
        //        type = type.Replace(">", "");

        //        if (listOfSimilarClasses.Any(x => x.DOClass.FullName == type) ||
        //            listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.FullName == type))
        //        {
        //            return KindOfProperty.CollectionAttributeClasses;
        //        }

        //        if (listOfSimilarClasses.Any(x => x.DtoClass.FullName == type) ||
        //            listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.FullName == type))
        //        {
        //            return KindOfProperty.CollectionAttributeClasses;
        //        }
        //    }

        //    if (listOfSimilarClasses.Any(x => x.DOClass.FullName == type) ||
        //        listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.FullName == type))
        //    {
        //        return KindOfProperty.AttributeClass;
        //    }

        //    if (listOfSimilarClasses.Any(x => x.DtoClass.FullName == type) ||
        //        listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.FullName == type))
        //    {
        //        return KindOfProperty.AttributeClass;
        //    }

        //    return KindOfProperty.None;
        //}

        public async Task<IEnumerable<ClassDeclarationSyntax>> GetAllClasses(string project, bool isSkipAttribute,string attribute)
        {
            _project = _solution.Projects.First(x => x.Name == project);
            var compilation = await _project.GetCompilationAsync();
            var classVisitor = new ClassVirtualizationVisitor();
            var classes = new List<ClassDeclarationSyntax>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                classVisitor.Visit(syntaxTree.GetRoot());
            }

            if (!isSkipAttribute)
            {
                classes = classVisitor.Classes.Where(x => x.AttributeLists
                    .Any(att => att.Attributes
                        .Any(att2 => att2.Name.ToString() == attribute))).ToList();
            }
            else
            {
                classes = classVisitor.Classes;
            }

            return classes;
        }


        public async void GenerateMap(MappingsGenerator MapHelper)
        {
            StringBuilder sb = new StringBuilder();
            _solution = await _workspace.OpenSolutionAsync(SolutionPath);

            if (string.IsNullOrEmpty(MapHelper.MapAttribute))
            {
                MapHelper.MapAttribute = "Map";
            }

            if (string.IsNullOrEmpty(MapHelper.MapIgnoreAttribute))
            {
                MapHelper.MapIgnoreAttribute = "MapIgnore";
            }

            if (MapHelper.DoProjects == null || !MapHelper.DoProjects.Any())
            {
                throw new Exception("List of DoProjects doesn't exist");
            }

            if (MapHelper.DtoProjects == null || !MapHelper.DtoProjects.Any())
            {
                throw new Exception("List of DtoProjects doesn't exist");
            }

            if (string.IsNullOrEmpty(MapHelper.MapExtensionClassName))
            {
                throw new Exception("Name of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(MapHelper.MapExtensionNameSpace))
            {
                throw new Exception("Namespace of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(MapHelper.MapAttribute))
            {
                throw new Exception("Attribute for Mapping doesn't exist");
            }

            var classesWithMapAttribute = new List<ClassDeclarationSyntax>();
            var classesWithMapAttributeDto = new List<ClassDeclarationSyntax>();

            foreach (string project in MapHelper.DoProjects)
            {
                classesWithMapAttribute.AddRange(
                    await GetAllClasses(project, MapHelper.DOSkipAttribute, MapHelper.MapAttribute));
            }

            foreach (string project in MapHelper.DtoProjects)
            {
                classesWithMapAttributeDto.AddRange(
                    await GetAllClasses(project, MapHelper.DOSkipAttribute, MapHelper.MapAttribute));
            }

            sb.Append("using System; \nusing System.Linq;\nusing YumaPos.Shared.API.Enums;\n\n");
            sb.Append("namespace " + MapHelper.MapExtensionNameSpace + "\n{\n");
            sb.Append("\t public static class " + MapHelper.MapExtensionClassName + "\n\t {\n");

            var listOfSimilarClasses = new List<MapDtoAndDo>();
            var isPairFounded = false;

            while (classesWithMapAttribute.Any())
            {
                isPairFounded = false;
                ClassDeclarationSyntax doClass = classesWithMapAttribute.First();
                foreach (ClassDeclarationSyntax dtoClass in classesWithMapAttributeDto)
                {
                    InterfaceDeclarationSyntax doInterface = null;
                    InterfaceDeclarationSyntax dtoInterface = null;

                    //doClass - КАК ПОЛУЧАТЬ У НЕГО ПРОПЕРТИ И МЕТОДЫ!

                    //if (MapHelper.GetMapName(doClass) == MapHelper.GetMapName(dtoClass) ||
                    //    (MapHelper.DOSkipAttribute && MapHelper.GetMapName(dtoClass) == doClass.Identifier.ToString()) ||
                    //    (MapHelper.DTOSkipAttribute && MapHelper.GetMapName(doClass) == dtoClass.Identifier.ToString()))
                    //{

                    if (doClass == dtoClass)
                    {
                        var compilation = CSharpCompilation.Create("MyCompilation",
                            syntaxTrees: new[] {doClass.SyntaxTree});
                        var model = compilation.GetSemanticModel(doClass.SyntaxTree);

                        var interfaces = doClass.SyntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                        
                        foreach (var inface in interfaces)
                        {
                            var myClassSymbol = model.GetDeclaredSymbol(inface);
                            var interfaceName = myClassSymbol.BaseType.Name;

                            if (interfaceName.IndexOf("I", StringComparison.Ordinal) == 0 &&
                                doClass.Identifier.ToString() == inface.Identifier.ToString().Remove(0, 1))
                            {
                                doInterface = inface;
                            }
                        }

                        compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] {dtoClass.SyntaxTree});
                        model = compilation.GetSemanticModel(doClass.SyntaxTree);

                        interfaces =
                            dtoClass.SyntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                        foreach (var inface in interfaces)
                        {
                            var myClassSymbol = model.GetDeclaredSymbol(inface);
                            var interfaceName = myClassSymbol.BaseType.Name;

                            if (interfaceName.IndexOf("I", StringComparison.Ordinal) == 0 &&
                                dtoClass.Identifier.ToString() == inface.Identifier.ToString().Remove(0, 1))
                            {
                                dtoInterface = inface;
                            }
                        }

                        listOfSimilarClasses.Add(new MapDtoAndDo
                        {
                            DOClass = doClass,
                            DtoClass = dtoClass,
                            DOInterface = doInterface,
                            DtoInterface = dtoInterface
                        });

                        classesWithMapAttributeDto.Remove(dtoClass);
                        isPairFounded = true;
                        break;
                    }
                }
                classesWithMapAttribute.Remove(doClass);
                if (!isPairFounded && !MapHelper.DOSkipAttribute && !MapHelper.DTOSkipAttribute)
                    sb.Append("\r\n//" + doClass.Identifier);
            }

            if (!MapHelper.DOSkipAttribute && !MapHelper.DTOSkipAttribute)
            {
                foreach (var dtoClass in classesWithMapAttributeDto)
                {
                    sb.Append("\r\n//" + dtoClass.Identifier);
                }
            }
        }
    }
}

//            foreach (var similarClass in listOfSimilarClasses)
//            {
//                var allProperties = similarClass.DOClass.Members!!!!
//                    GetAllCodeElementsOfType(similarClass.DOClass.Members, vsCMElement.vsCMElementProperty, true);
//                var CECollection = similarClass.DOClass.Bases;
//                foreach (CodeClass ce in CECollection) allProperties.AddRange(VisualStudioHelper.GetAllCodeElementsOfType(ce.Members, vsCMElement.vsCMElementProperty, true));

//                var allPropertiesDto = VisualStudioHelper.GetAllCodeElementsOfType(similarClass.DtoClass.Members, vsCMElement.vsCMElementProperty, true);
//                CECollection = similarClass.DtoClass.Bases;
//                foreach (CodeClass ce in CECollection) allPropertiesDto.AddRange(VisualStudioHelper.GetAllCodeElementsOfType(ce.Members, vsCMElement.vsCMElementProperty, true));

//                var allMethods = VisualStudioHelper.GetAllCodeElementsOfType(similarClass.DOClass.Members, vsCMElement.vsCMElementFunction, true);
//                foreach (CodeClass ce in CECollection) allMethods.AddRange(VisualStudioHelper.GetAllCodeElementsOfType(ce.Members, vsCMElement.vsCMElementFunction, true));

//                var allMethodsDto = VisualStudioHelper.GetAllCodeElementsOfType(similarClass.DtoClass.Members, vsCMElement.vsCMElementFunction, true);
//                foreach (CodeClass ce in CECollection) allMethodsDto.AddRange(VisualStudioHelper.GetAllCodeElementsOfType(ce.Members, vsCMElement.vsCMElementFunction, true));

//                var listOfSimilarProperties = new List<MapPropertiesDtoAndDo>();

//                var isIgnoreDOProperties = VisualStudioHelper.GetAllCodeElementsOfType(similarClass.DOClass.Members, vsCMElement.vsCMElementProperty, true);
//                var isIgnoreDTOProperties = VisualStudioHelper.GetAllCodeElementsOfType(similarClass.DtoClass.Members, vsCMElement.vsCMElementProperty, true);

//                while (allProperties.Any())
//                {
//                    var attrIgnoreDo = new AttributeAndPropeperties();
//                    attrIgnoreDo = null;

//                    CodeProperty codeProperty = (CodeProperty)allProperties.First();
//                    var str = codeProperty.Name;
//                    if (MapHelper.MapIgnoreAttribute != null && MapHelper.MapIgnoreAttribute != "")
//                    {
//                        attrIgnoreDo = VisualStudioHelper.GetAttributesAndPropepertiesCollection((CodeElement)codeProperty).FirstOrDefault(x => x.Name == MapHelper.MapIgnoreAttribute);
//                    }

//                    foreach (CodeProperty dtoCodeProperty in allPropertiesDto)
//                    {
//                        var str1 = dtoCodeProperty.Name;
//                        var attrIgnoreDto = new AttributeAndPropeperties();
//                        attrIgnoreDto = null;

//                        if (MapHelper.MapIgnoreAttribute != null && MapHelper.MapIgnoreAttribute != "")
//                        {
//                            attrIgnoreDto = VisualStudioHelper.GetAttributesAndPropepertiesCollection((CodeElement)dtoCodeProperty).FirstOrDefault(x => x.Name == MapHelper.MapIgnoreAttribute);
//                        }

//                        if (MapHelper.GetMapName((CodeElement)codeProperty) == MapHelper.GetMapName((CodeElement)dtoCodeProperty))
//                        {
//                            var doPropertyName = codeProperty.Name;
//                            var dtoPropertyName = dtoCodeProperty.Name;

//                            AttributeAndPropeperties attrDo = VisualStudioHelper.GetAttributesAndPropepertiesCollection((CodeElement)codeProperty).FirstOrDefault(x => x.Name == MapHelper.MapAttribute);
//                            AttributeAndPropeperties attrDto = VisualStudioHelper.GetAttributesAndPropepertiesCollection((CodeElement)dtoCodeProperty).FirstOrDefault(x => x.Name == MapHelper.MapAttribute);

//                            var FunctionFromDto = attrDto != null ? attrDto.GetParameterByKeyName("FunctionFrom") : null;
//                            var FunctionFrom = attrDo != null ? attrDo.GetParameterByKeyName("FunctionFrom") : null;
//                            var FunctionToDto = attrDto != null ? attrDto.GetParameterByKeyName("FunctionTo") : null;
//                            var FunctionTo = attrDo != null ? attrDo.GetParameterByKeyName("FunctionTo") : null;

//                            var FromDtoFunction = "";
//                            var ToDtoFunction = "";

//                            if (FunctionFromDto != null)
//                            {
//                                if (FunctionTo != null)
//                                {
//                                    if (FunctionFromDto != FunctionTo)
//                                    {
//                                        throw new Exception("In the pair: " + similarClass.DOClass.Name + " " + similarClass.DtoClass.Name + " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
//                                    }
//                                }

//                                FromDtoFunction = FunctionFromDto;
//                            }
//                            else
//                            {
//                                if (FunctionTo != null)
//                                {
//                                    FromDtoFunction = FunctionTo;
//                                }
//                            }

//                            if (FunctionToDto != null)
//                            {
//                                if (FunctionFrom != null)
//                                {
//                                    if (FunctionToDto != FunctionFrom)
//                                    {
//                                        throw new Exception("In the pair: " + similarClass.DOClass.Name + " " + similarClass.DtoClass.Name + " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
//                                    }
//                                }

//                                ToDtoFunction = FunctionToDto;
//                            }
//                            else
//                            {
//                                if (FunctionFrom != null)
//                                {
//                                    ToDtoFunction = FunctionFrom;
//                                }
//                            }

//                            if (!MapHelper.CompareTwoPropertyType(codeProperty, dtoCodeProperty, listOfSimilarClasses) && ToDtoFunction == "" && FromDtoFunction == "")
//                            {
//                                if (MapHelper.IsInNullableDictionary(codeProperty.Type.AsString, dtoCodeProperty.Type.AsString))
//                                {
//                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".HasValue ? itemDto." + dtoCodeProperty.Name + ".Value : default(" + codeProperty.Type.AsString + ")";
//                                    ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
//                                }

//                                if (MapHelper.IsInNullableDictionary(dtoCodeProperty.Type.AsString, codeProperty.Type.AsString))
//                                {
//                                    ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".HasValue ? item." + codeProperty.Name + ".Value : default(" + dtoCodeProperty.Type.AsString + ")";
//                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
//                                }
//                            }

//                            var isSetterDO = VisualStudioHelper.IsExistSetterInCodeProperty(codeProperty);
//                            var isSetterDto = VisualStudioHelper.IsExistSetterInCodeProperty(dtoCodeProperty);
//                            bool ignoreBySetter = false;

//                            if (ToDtoFunction == "" && FromDtoFunction == "")
//                            {
//                                if (codeProperty.Type.AsString.Contains("ObservableRangeCollection") || dtoCodeProperty.Type.AsString.Contains("ObservableRangeCollection"))
//                                {
//                                    var typeOfObservableCollectionDO = codeProperty.Type.AsString.Remove(0, codeProperty.Type.AsString.IndexOf("<") + 1);
//                                    typeOfObservableCollectionDO = typeOfObservableCollectionDO.Replace(">", "");

//                                    var typeOfObservableCollectionDto = dtoCodeProperty.Type.AsString.Remove(0, dtoCodeProperty.Type.AsString.IndexOf("<") + 1);
//                                    typeOfObservableCollectionDto = typeOfObservableCollectionDto.Replace(">", "");

//                                    var kindOfProperty = MapHelper.GetKindOfMapProperty(typeOfObservableCollectionDO, listOfSimilarClasses);

//                                    if (MapHelper.CompareTwoPropertyType(typeOfObservableCollectionDO, typeOfObservableCollectionDto, listOfSimilarClasses))
//                                    {
//                                        if (codeProperty.Type.AsString.Contains("ObservableRangeCollection"))
//                                        {
//                                            if (kindOfProperty == KindOfProperty.AttributeClass)
//                                            {
//                                                FromDtoFunction = "if(itemDto." + dtoCodeProperty.Name + " != null) item." + codeProperty.Name + ".ReplaceRange(itemDto." + dtoCodeProperty.Name + ".Select(x => x.MapFromDto()))";
//                                                ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = item." + codeProperty.Name + " != null" + " ? item." + codeProperty.Name + ".Select(x => x.MapToDto()) : null";
//                                            }
//                                            else
//                                            {
//                                                FromDtoFunction = "if(itemDto." + dtoCodeProperty.Name + " != null) item." + codeProperty.Name + ".ReplaceRange(itemDto." + dtoCodeProperty.Name + ")";
//                                                ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
//                                            }
//                                        }

//                                        if (dtoCodeProperty.Type.AsString.Contains("ObservableRangeCollection"))
//                                        {
//                                            if (kindOfProperty == KindOfProperty.AttributeClass)
//                                            {
//                                                ToDtoFunction = "if(item." + codeProperty.Name + " != null) itemDto." + dtoCodeProperty.Name + ".ReplaceRange(item." + codeProperty.Name + ".Select(x => x.MapToDto()))";
//                                                if (FromDtoFunction == "")
//                                                {
//                                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
//                                                }
//                                            }
//                                            else
//                                            {
//                                                ToDtoFunction = "if(item." + codeProperty.Name + " != null) itemDto." + dtoCodeProperty.Name + ".ReplaceRange(item." + codeProperty.Name + ")";
//                                                if (FromDtoFunction == "")
//                                                {
//                                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }

//                            if (!isSetterDO && !codeProperty.Type.AsString.Contains("ObservableRangeCollection"))
//                            {
//                                ignoreBySetter = true;
//                            }

//                            if (!isSetterDto && !dtoCodeProperty.Type.AsString.Contains("ObservableRangeCollection"))
//                            {
//                                ignoreBySetter = true;
//                            }

//                            if (MapHelper.CompareTwoPropertyType(codeProperty, dtoCodeProperty, listOfSimilarClasses) || ToDtoFunction != "" || FromDtoFunction != "")
//                            {
//                                var kindOfProperty = MapHelper.GetKindOfMapProperty(codeProperty, listOfSimilarClasses);

//                                if (ToDtoFunction == "" && FromDtoFunction == "")
//                                {
//                                    if (kindOfProperty == KindOfProperty.AttributeClass)
//                                    {
//                                        ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".MapToDto()";
//                                        FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".MapFromDto()";
//                                    }
//                                    else
//                                    {
//                                        if (kindOfProperty == KindOfProperty.CollectionAttributeClasses)
//                                        {
//                                            ToDtoFunction = "if(item." + codeProperty.Name + " != null) itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".Select(x => x.MapToDto())";
//                                            FromDtoFunction = "if(itemDto." + codeProperty.Name + " != null) item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".Select(x => x.MapFromDto())";
//                                        }
//                                        else
//                                        {
//                                            ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
//                                            FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
//                                        }
//                                    }
//                                }

//                                if (attrIgnoreDo == null && attrIgnoreDto == null && !ignoreBySetter)
//                                {
//                                    listOfSimilarProperties.Add(new MapPropertiesDtoAndDo
//                                    {
//                                        DOPropertyName = codeProperty,
//                                        DTOPropertyName = dtoCodeProperty,
//                                        DOPropertyType = codeProperty.Type,
//                                        DtoropertyType = dtoCodeProperty.Type,
//                                        KindOMapfProperty = kindOfProperty,
//                                        FromDtoFunction = FromDtoFunction,
//                                        ToDtoFunction = ToDtoFunction
//                                    });
//                                    isIgnoreDOProperties.Remove((CodeElement)codeProperty);
//                                    isIgnoreDTOProperties.Remove((CodeElement)dtoCodeProperty);
//                                }

//                                if (attrIgnoreDto != null)
//                                {
//                                    isIgnoreDTOProperties.Remove((CodeElement)dtoCodeProperty);
//                                }

//                                allPropertiesDto.Remove((CodeElement)dtoCodeProperty);
//                                break;
//                            }
//                        }
//                    }
//                    if (attrIgnoreDo != null)
//                    {
//                        isIgnoreDOProperties.Remove((CodeElement)codeProperty);
//                    }
//                    allProperties.Remove((CodeElement)codeProperty);
//                }

//                similarClass.MapPropertiesDtoAndDo = listOfSimilarProperties;

//    #>    
//        public static <#= similarClass.DtoInterface == null ? similarClass.DtoClass.FullName : similarClass.DtoInterface.FullName #> MapToDto (this <#= similarClass.DOInterface == null ? similarClass.DOClass.FullName : similarClass.DOInterface.FullName #> item)
//        {  
//<#+         foreach (var prop in isIgnoreDOProperties)
//            {
//#>            //itemDo.<#= prop.Name #>
//<#+			}
//#>
//            if (item == null) return null;
            
//            var itemDto = new <#= similarClass.DtoClass.FullName #> ();
//<#+         foreach(MapPropertiesDtoAndDo property in similarClass.MapPropertiesDtoAndDo)
//            { 
//                if(property.ToDtoFunction != "" ) 
//                {
//#>                <#= property.ToDtoFunction #>;
//<#+            }
//            } #>

//            return itemDto;
//        }

//    public static <#= similarClass.DOInterface == null ? similarClass.DOClass.FullName : similarClass.DOInterface.FullName #> MapFromDto (this <#= similarClass.DtoInterface == null ? similarClass.DtoClass.FullName : similarClass.DtoInterface.FullName #> itemDto)
//        {  
//<#+         foreach (var prop in isIgnoreDTOProperties)
//            {
//#>            //itemDto.<#= prop.Name #>
//<#+			}
//#>
//            if (itemDto == null) return null;
            
//            var item = new <#= similarClass.DOClass.FullName #> ();
//<#+          foreach(MapPropertiesDtoAndDo property in similarClass.MapPropertiesDtoAndDo)
//            { 
//                if(property.FromDtoFunction != "" )
//                {
//#>                <#= property.FromDtoFunction #>;
//<#+             }
//            } #>
            
//            return item;
//        }
//    <#+  } #>

//    }
//}
//<#+  }
//        }
//    }
//}


