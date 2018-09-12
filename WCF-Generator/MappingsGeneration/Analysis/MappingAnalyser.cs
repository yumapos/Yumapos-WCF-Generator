using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Infrastructure;

namespace WCFGenerator.MappingsGenerator.Analysis
{
    public class MappingAnalyser
    {
        private readonly MappingConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;
        private Solution _solution = null;

        public IList<ClassCompilerInfo> ClassesWithoutPair = new List<ClassCompilerInfo>();
        public IList<MapDtoAndDo> ListOfSimilarClasses;

        public MappingAnalyser(MappingConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
            _solution = _generatorWorkspace.Solution;
        }

        public async Task Run()
        {
            StringBuilder sb = new StringBuilder();

            if (_configuration.DoProjects == null || _configuration.DoProjects.Count == 0)
            {
                throw new Exception("List of DoProjects doesn't exist");
            }

            if (_configuration.DtoProjects == null || _configuration.DoProjects.Count == 0)
            {
                throw new Exception("List of DtoProjects doesn't exist");
            }

            if (string.IsNullOrEmpty(_configuration.MapExtensionClassName))
            {
                throw new Exception("Name of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(_configuration.MapExtensionNameSpace))
            {
                throw new Exception("Namespace of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(_configuration.MapAttribute))
            {
                throw new Exception("Attribute for Mapping doesn't exist");
            }

            var classesWithMapAttribute = new List<ClassCompilerInfo>();
            var classesWithMapAttributeDto = new List<ClassCompilerInfo>();

            foreach (MappingSourceProject project in _configuration.DoProjects)
            {
                classesWithMapAttribute.AddRange(
                    await GetAllClasses(project.ProjectName, _configuration.DOSkipAttribute, _configuration.MapAttribute));
            }

            foreach (MappingSourceProject project in _configuration.DtoProjects)
            {
                classesWithMapAttributeDto.AddRange(
                    await GetAllClasses(project.ProjectName, _configuration.DOSkipAttribute, _configuration.MapAttribute));
            }

            var listOfSimilarClasses = new List<MapDtoAndDo>();
            var isPairFound = false;
            while (classesWithMapAttribute.Any())
            {
                isPairFound = false;
                var doClass = classesWithMapAttribute.First();
                foreach (var dtoClass in classesWithMapAttributeDto)
                {
                    INamedTypeSymbol doInterface = null;
                    INamedTypeSymbol dtoInterface = null;

                    if (GetMapNameForClass(doClass.NamedTypeSymbol) == GetMapNameForClass(dtoClass.NamedTypeSymbol) ||
                        (_configuration.DOSkipAttribute && GetMapNameForClass(dtoClass.NamedTypeSymbol) == doClass.NamedTypeSymbol.Name) ||
                        (_configuration.DTOSkipAttribute && GetMapNameForClass(doClass.NamedTypeSymbol) == dtoClass.NamedTypeSymbol.Name))
                    {
                        // TODO: recursion for getting base interfaces
                        foreach (var ce in doClass.NamedTypeSymbol.Interfaces)
                        {
                            if (ce.Name.IndexOf("I") == 0 && doClass.NamedTypeSymbol.Name == ce.Name.Remove(0, 1))
                            {
                                doInterface = ce;
                            }
                        }

                        // TODO: recursion
                        foreach (var ce in dtoClass.NamedTypeSymbol.Interfaces)
                        {
                            if (ce.Name.IndexOf("I") == 0 && dtoClass.NamedTypeSymbol.Name == ce.Name.Remove(0, 1))
                            {
                                dtoInterface = ce;
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
                        isPairFound = true;
                        break;
                    }
                }

                if (!isPairFound)
                {
                    ClassesWithoutPair.Add(doClass);
                }
                classesWithMapAttribute.Remove(doClass);
            }

            foreach (var dtoClass in classesWithMapAttributeDto)
            {
                ClassesWithoutPair.Add(dtoClass);
            }

            foreach (var similarClass in listOfSimilarClasses)
            {
                var allProperties = similarClass.DOClass.NamedTypeSymbol.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>().ToList();
                var baseClass = similarClass.DOClass.NamedTypeSymbol.BaseType;
                while (baseClass != null)
                {
                    allProperties.AddRange(baseClass.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>());
                    baseClass = baseClass.BaseType;
                }

                var allPropertiesDto = similarClass.DtoClass.NamedTypeSymbol.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>().ToList();
                baseClass = similarClass.DOClass.NamedTypeSymbol.BaseType;
                while (baseClass != null)
                {
                    allPropertiesDto.AddRange(baseClass.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>());
                    baseClass = baseClass.BaseType;
                }
                var listOfSimilarProperties = new List<MapPropertiesDtoAndDo>();

                var isIgnoreDOProperties = new List<IPropertySymbol>(allProperties.ToList()); // to remove items from it later
                var isIgnoreDTOProperties = new List<IPropertySymbol>(allPropertiesDto.ToList());
                while (allProperties.Any())
                {
                    AttributeData attrIgnoreDo = null;

                    var codeProperty = allProperties.First();
                    var str = codeProperty.Name;
                    if (_configuration.MapIgnoreAttribute != null && _configuration.MapIgnoreAttribute != "")
                    {
                        attrIgnoreDo = codeProperty.GetAttributeByName(_configuration.MapIgnoreAttribute);
                    }

                    foreach (var dtoCodeProperty in allPropertiesDto)
                    {
                        var str1 = dtoCodeProperty.Name;
                        AttributeData attrIgnoreDto = null;

                        if (_configuration.MapIgnoreAttribute != null && _configuration.MapIgnoreAttribute != "")
                        {
                            attrIgnoreDto = dtoCodeProperty.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == _configuration.MapIgnoreAttribute);
                        }

                        if (GetMapNameForProperty(codeProperty) == GetMapNameForProperty(dtoCodeProperty))
                        {
                            var doPropertyName = codeProperty.Name;
                            var dtoPropertyName = dtoCodeProperty.Name;

                            var attrDo = codeProperty.GetAttributeByName(_configuration.MapAttribute);
                            var attrDto = dtoCodeProperty.GetAttributeByName(_configuration.MapAttribute);

                            var FunctionFromDto = attrDto?.GetAttributePropertyValue("FunctionFrom");
                            var FunctionFrom = attrDo?.GetAttributePropertyValue("FunctionFrom");
                            var FunctionToDto = attrDto?.GetAttributePropertyValue("FunctionTo");
                            var FunctionTo = attrDo?.GetAttributePropertyValue("FunctionTo");

                            var FromDtoFunction = "";
                            var ToDtoFunction = "";

                            if (FunctionFromDto != null)
                            {
                                if (FunctionTo != null)
                                {
                                    if (FunctionFromDto != FunctionTo)
                                    {
                                        throw new Exception("In the pair: " + similarClass.DOClass.NamedTypeSymbol.Name + " " + similarClass.DtoClass.NamedTypeSymbol.Name + " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
                                    }
                                }

                                FromDtoFunction = FunctionFromDto;
                            }
                            else
                            {
                                if (FunctionTo != null)
                                {
                                    FromDtoFunction = FunctionTo;
                                }
                            }

                            if (FunctionToDto != null)
                            {
                                if (FunctionFrom != null)
                                {
                                    if (FunctionToDto != FunctionFrom)
                                    {
                                        throw new Exception("In the pair: " + similarClass.DOClass.NamedTypeSymbol.Name + " " + similarClass.DtoClass.NamedTypeSymbol.Name + " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
                                    }
                                }

                                ToDtoFunction = FunctionToDto;
                            }
                            else
                            {
                                if (FunctionFrom != null)
                                {
                                    ToDtoFunction = FunctionFrom;
                                }
                            }

                            if (!CompareTwoPropertyType(codeProperty, dtoCodeProperty, listOfSimilarClasses) && ToDtoFunction == "" && FromDtoFunction == "")
                            {
                                if (IsInNullableDictionary(codeProperty.Type.Name, dtoCodeProperty.Type.Name))
                                {
                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".HasValue ? itemDto." + dtoCodeProperty.Name + ".Value : default(" + codeProperty.Type.Name + ")";
                                    ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
                                }

                                if (IsInNullableDictionary(dtoCodeProperty.Type.Name, codeProperty.Type.Name))
                                {
                                    ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".HasValue ? item." + codeProperty.Name + ".Value : default(" + dtoCodeProperty.Type.Name + ")";
                                    FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
                                }
                            }

                            var isSetterDO = codeProperty.SetMethod != null;
                            var isSetterDto = dtoCodeProperty.SetMethod != null;
                            bool ignoreBySetter = false;

                            if (!isSetterDO)
                            {
                                ignoreBySetter = true;
                            }

                            if (!isSetterDto)
                            {
                                ignoreBySetter = true;
                            }

                            if (CompareTwoPropertyType(codeProperty, dtoCodeProperty, listOfSimilarClasses) || ToDtoFunction != "" || FromDtoFunction != "")
                            {
                                var kindOfProperty = GetKindOfMapProperty(codeProperty, listOfSimilarClasses);

                                if (ToDtoFunction == "" && FromDtoFunction == "")
                                {
                                    if (kindOfProperty == KindOfProperty.AttributeClass)
                                    {
                                        ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".MapToDto()";
                                        FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".MapFromDto()";
                                    }
                                    else
                                    {
                                        if (kindOfProperty == KindOfProperty.CollectionAttributeClasses)
                                        {
                                            ToDtoFunction = "if(item." + codeProperty.Name + " != null) itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".Select(x => x.MapToDto())";
                                            FromDtoFunction = "if(itemDto." + codeProperty.Name + " != null) item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".Select(x => x.MapFromDto())";
                                        }
                                        else
                                        {
                                            ToDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
                                            FromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
                                        }
                                    }
                                }

                                if (attrIgnoreDo == null && attrIgnoreDto == null && !ignoreBySetter)
                                {
                                    listOfSimilarProperties.Add(new MapPropertiesDtoAndDo
                                    {
                                        DOPropertyName = codeProperty,
                                        DTOPropertyName = dtoCodeProperty,
                                        DOPropertyType = codeProperty.Type,
                                        DtoropertyType = dtoCodeProperty.Type,
                                        KindOMapfProperty = kindOfProperty,
                                        FromDtoFunction = FromDtoFunction,
                                        ToDtoFunction = ToDtoFunction
                                    });
                                    isIgnoreDOProperties.Remove(codeProperty);
                                    isIgnoreDTOProperties.Remove(dtoCodeProperty);
                                }

                                if (attrIgnoreDto != null)
                                {
                                    isIgnoreDTOProperties.Remove(dtoCodeProperty);
                                }

                                allPropertiesDto.Remove(dtoCodeProperty);
                                break;
                            }
                        }
                    }
                    if (attrIgnoreDo != null)
                    {
                        isIgnoreDOProperties.Remove(codeProperty);
                    }
                    allProperties.Remove(codeProperty);
                }

                similarClass.MapPropertiesDtoAndDo = listOfSimilarProperties;
                similarClass.IsIgnoreDOProperties = isIgnoreDOProperties.ToArray();
                similarClass.IsIgnoreDTOProperties = isIgnoreDTOProperties.ToArray();
            }

            ListOfSimilarClasses = listOfSimilarClasses;
        }

        public async Task<IEnumerable<ClassCompilerInfo>> GetAllClasses(string projectName, bool isSkipAttribute, string attribute)
        {
            var project = _solution.Projects.First(x => x.Name == projectName);
            var compilation = (CSharpCompilation)(await project.GetCompilationAsync());
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

            var ret = new List<ClassCompilerInfo>();

            foreach (var classDeclarationSyntax in classes)
            {
                var typeInfo = compilation.GetClass(classDeclarationSyntax);
                ret.Add(new ClassCompilerInfo()
                {
                    ClassDeclarationSyntax = classDeclarationSyntax,
                    NamedTypeSymbol = typeInfo
                });
            }

            return ret;
        }

        public string GetMapNameForProperty(IPropertySymbol propertySymbol)
        {
            string value = propertySymbol.Name;
            const string nameProperty = "Name";

            var attributes = propertySymbol.GetAttributes();

            foreach (var ca in attributes)
            {
                if (ca.AttributeClass.Name.Contains(_configuration.MapAttribute) && ca.NamedArguments.Any(a => a.Key.Contains(nameProperty)))
                {
                    value = ca.NamedArguments.First(a => a.Key.Contains(nameProperty)).Value.Value.ToString();
                    break;
                }
            }

            value = value.ToLower();
            return value;
        }

        public string GetMapNameForClass(INamedTypeSymbol element)
        {
            string value = element.Name;
            const string nameProperty = "Name";

            var attributes = element.GetAttributes();

            foreach (var ca in attributes)
            {
                if (ca.AttributeClass.Name.Contains(_configuration.MapAttribute) && ca.NamedArguments.Any(a => a.Key.Contains(nameProperty)))
                {
                    value = ca.NamedArguments.First(a => a.Key.Contains(nameProperty)).Value.Value.ToString();
                    if (!String.IsNullOrEmpty(_configuration.DoSuffix))
                    {
                        if (value.EndsWith(_configuration.DoSuffix.ToLower()))
                        {
                            value = value.Replace(_configuration.DoSuffix.ToLower(), "");
                        }
                    }

                    if (!String.IsNullOrEmpty(_configuration.DtoSuffix))
                    {
                        if (value.EndsWith(_configuration.DtoSuffix.ToLower()))
                        {
                            value = value.Replace(_configuration.DtoSuffix.ToLower(), "");
                        }
                    }
                }
            }

            value = value.ToLower();

            if (!String.IsNullOrEmpty(_configuration.DoSuffix))
            {
                if (value.EndsWith(_configuration.DoSuffix.ToLower()))
                {
                    value = value.Replace(_configuration.DoSuffix.ToLower(), "");
                }
            }

            if (!String.IsNullOrEmpty(_configuration.DtoSuffix))
            {
                if (value.EndsWith(_configuration.DtoSuffix.ToLower()))
                {
                    value = value.Replace(_configuration.DtoSuffix.ToLower(), "");
                }
            }

            return value;
        }

        public bool CompareTwoPropertyType(IPropertySymbol DOCodeProperty, IPropertySymbol DtoCodeProperty, List<MapDtoAndDo> similarClasses)
        {
            var DOType = DOCodeProperty.Type.GetFullName();
            var DtoType = DtoCodeProperty.Type.GetFullName();

            if (DOType.Contains("IEnumerable") && DtoType.Contains("IEnumerable"))
            {
                DOType = DOType.Remove(0, DOType.IndexOf("<") + 1);
                DOType = DOType.Replace(">", "");

                DtoType = DtoType.Remove(0, DtoType.IndexOf("<") + 1);
                DtoType = DtoType.Replace(">", "");

                if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
                    return true;
            }

            if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
                return true;

            if (DOType == DtoType)
            {
                return true;
            }

            return false;
        }

        public bool CompareTwoPropertyType(string DOType, string DtoType, List<MapDtoAndDo> similarClasses)
        {
            if (DOType.Contains("IEnumerable") && DtoType.Contains("IEnumerable"))
            {
                DOType = DOType.Remove(0, DOType.IndexOf("<") + 1);
                DOType = DOType.Replace(">", "");

                DtoType = DtoType.Remove(0, DtoType.IndexOf("<") + 1);
                DtoType = DtoType.Replace(">", "");

                if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
                    return true;
            }

            if (CompareInMapDtoAndDoCollection(DOType, DtoType, similarClasses))
                return true;

            if (DOType == DtoType)
            {
                return true;
            }

            return false;
        }

        public bool CompareInMapDtoAndDoCollection(string DOType, string DtoType, List<MapDtoAndDo> similarClasses)
        {
            var similarInterfacesDO = similarClasses.Where(x => x.DOInterface != null).FirstOrDefault(x => x.DOInterface.GetFullName() == DOType);
            var similarInterfacesDto = similarClasses.Where(x => x.DtoInterface != null).FirstOrDefault(x => x.DtoInterface.GetFullName() == DtoType);
            var checkingMapClass = new MapDtoAndDo();

            checkingMapClass = similarClasses.FirstOrDefault(x => x.DOClass.NamedTypeSymbol.GetFullName() == DOType);
            if (checkingMapClass != null && checkingMapClass == similarClasses.FirstOrDefault(x => x.DtoClass.NamedTypeSymbol.GetFullName() == DtoType))
            {
                return true;
            }

            similarInterfacesDO = similarClasses.Where(x => x.DOInterface != null).FirstOrDefault(x => x.DOInterface.GetFullName() == DOType);
            similarInterfacesDto = similarClasses.Where(x => x.DtoInterface != null).FirstOrDefault(x => x.DtoInterface.GetFullName() == DtoType);

            if (similarInterfacesDO != null && similarInterfacesDO == similarClasses.FirstOrDefault(x => x.DtoClass.NamedTypeSymbol.GetFullName() == DtoType))
            {
                return true;
            }

            if (similarInterfacesDto != null && similarClasses.FirstOrDefault(x => x.DOClass.NamedTypeSymbol.GetFullName() == DOType) == similarInterfacesDto)
            {
                return true;
            }

            if (similarInterfacesDO != null && similarInterfacesDO == similarInterfacesDto)
            {
                return true;
            }

            return false;
        }

        public KindOfProperty GetKindOfMapProperty(IPropertySymbol codeProperty, List<MapDtoAndDo> listOfSimilarClasses)
        {
            var type = codeProperty.Type.GetFullName();

            if (type.Contains("IEnumerable"))
            {
                type = type.Remove(0, type.IndexOf("<") + 1);
                type = type.Replace(">", "");

                if (listOfSimilarClasses.Any(x => x.DOClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.GetFullName() == type))
                {
                    return KindOfProperty.CollectionAttributeClasses;
                }

                if (listOfSimilarClasses.Any(x => x.DtoClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.GetFullName() == type))
                {
                    return KindOfProperty.CollectionAttributeClasses;
                }
            }

            if (listOfSimilarClasses.Any(x => x.DOClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.GetFullName() == type))
            {
                return KindOfProperty.AttributeClass;
            }

            if (listOfSimilarClasses.Any(x => x.DtoClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.GetFullName() == type))
            {
                return KindOfProperty.AttributeClass;
            }

            return KindOfProperty.None;
        }

        public KindOfProperty GetKindOfMapProperty(string type, List<MapDtoAndDo> listOfSimilarClasses)
        {
            if (type.Contains("IEnumerable"))
            {
                type = type.Remove(0, type.IndexOf("<") + 1);
                type = type.Replace(">", "");

                if (listOfSimilarClasses.Any(x => x.DOClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.GetFullName() == type))
                {
                    return KindOfProperty.CollectionAttributeClasses;
                }

                if (listOfSimilarClasses.Any(x => x.DtoClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.GetFullName() == type))
                {
                    return KindOfProperty.CollectionAttributeClasses;
                }
            }

            if (listOfSimilarClasses.Any(x => x.DOClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DOInterface != null).Any(x => x.DOInterface.GetFullName() == type))
            {
                return KindOfProperty.AttributeClass;
            }

            if (listOfSimilarClasses.Any(x => x.DtoClass.NamedTypeSymbol.GetFullName() == type) || listOfSimilarClasses.Where(x => x.DtoInterface != null).Any(x => x.DtoInterface.GetFullName() == type))
            {
                return KindOfProperty.AttributeClass;
            }

            return KindOfProperty.None;
        }

        public Dictionary<string, string[]> SystemNullableTypes = new Dictionary<string, string[]>
        {
        {"int", new string[]{"int?", "System.Int32?", "System.Nullable<int>", "System.Nullable<System.Int32>"}},
        {"System.Int32",new string[]{"int?", "System.Int32?", "System.Nullable<int>", "System.Nullable<System.Int32>"}},

        {"short", new string[]{"short?", "System.Int16?", "System.Nullable<short>", "System.Nullable<System.Int16>"}},
        {"System.Int16", new string[]{"short?", "System.Int16?", "System.Nullable<short>", "System.Nullable<System.Int16>"}},

        {"long", new string[]{"long?", "System.Int64?", "System.Nullable<long>", "System.Nullable<System.Int64>"}},
        {"System.Int64", new string[]{"long?", "System.Int64?", "System.Nullable<long>", "System.Nullable<System.Int64>"}},

        {"decimal", new string[]{"decimal?", "System.Decimal?", "System.Nullable<decimal>", "System.Nullable<System.Decimal>"}},
        {"System.Decimal", new string[]{"decimal?", "System.Decimal?", "System.Nullable<decimal>", "System.Nullable<System.Decimal>"}},

        {"float", new string[]{"float?", "System.Single?", "System.Nullable<float>", "System.Nullable<System.Single>"}},
        {"System.Single", new string[]{"float?", "System.Single?", "System.Nullable<float>", "System.Nullable<System.Single>"}},

        {"double", new string[]{"double?", "System.Double?", "System.Nullable<double>", "System.Nullable<System.Double>"}},
        {"System.Double", new string[]{"double?", "System.Double?", "System.Nullable<double>", "System.Nullable<System.Double>"}},

        {"bool", new string[]{"bool?", "System.Boolean?", "System.Nullable<bool>", "System.Nullable<System.Boolean>"}},
        {"System.Boolean", new string[]{"bool?", "System.Boolean?", "System.Nullable<bool>", "System.Nullable<System.Boolean>"}},

        {"byte", new string[]{"byte?", "System.Byte?", "System.Nullable<byte>", "System.Nullable<System.Byte>"}},
        {"System.Byte", new string[]{"byte?", "System.Byte?", "System.Nullable<byte>", "System.Nullable<System.Byte>"}},

        {"Guid", new string[]{"Guid?", "System.Guid?", "System.Nullable<Guid>", "System.Nullable<System.Guid>"}},
        {"System.Guid", new string[]{"Guid?", "System.Guid?", "System.Nullable<Guid>", "System.Nullable<System.Guid>"}},

        {"char", new string[]{"char?", "System.Char?", "System.Nullable<char>", "System.Nullable<System.Char>"}},
        {"System.Char", new string[]{"char?", "System.Char?", "System.Nullable<char>", "System.Nullable<System.Char>"}},

        {"DateTime", new string[]{"DateTime?", "System.DateTime?", "System.Nullable<DateTime>", "System.Nullable<System.DateTime>"}},
        {"System.DateTime", new string[]{"DateTime?", "System.DateTime?", "System.Nullable<DateTime>", "System.Nullable<System.DateTime>"}},

        {"sbyte", new string[]{"sbyte?", "System.SByte?", "System.Nullable<sbyte>", "System.Nullable<System.SByte>"}},
        {"System.SByte", new string[]{"sbyte?", "System.SByte?", "System.Nullable<sbyte>", "System.Nullable<System.SByte>"}},

        {"uint", new string[]{"uint?", "System.UInt32?", "System.Nullable<uint>", "System.Nullable<System.UInt32>"}},
        {"System.UInt32", new string[]{"uint?", "System.UInt32?", "System.Nullable<uint>", "System.Nullable<System.UInt32>"}},

        {"ulong", new string[]{"ulong?", "System.UInt64?", "System.Nullable<ulong>", "System.Nullable<System.UInt64>"}},
        {"System.UInt64", new string[]{"ulong?", "System.UInt64?", "System.Nullable<ulong>", "System.Nullable<System.UInt64>"}},

        {"ushort", new string[]{"ushort?", "System.UInt16?", "System.Nullable<ushort>", "System.Nullable<System.UInt16>"}},
        {"System.UInt16", new string[]{"ushort?", "System.UInt16?", "System.Nullable<ushort>", "System.Nullable<System.UInt16>"}},
        };

        public bool IsInNullableDictionary(string systemtype, string nullableType)
        {
            var nullableCollectionValue = SystemNullableTypes.FirstOrDefault(x => x.Key.ToString().Trim() == systemtype).Value;

            if (nullableCollectionValue != null)
            {
                var nullableCollection = nullableCollectionValue.ToList();
                if (nullableCollection.FirstOrDefault(x => x == nullableType) != null)
                    return true;
            }

            return false;
        }
    }
}
