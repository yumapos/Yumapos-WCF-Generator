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
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.DecoratorGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Configuration;
using WCFGenerator.MappingsGeneration.Infrastructure;

namespace WCFGenerator.MappingsGeneration.Analysis
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
            Validate();
            var listOfSimilarClasses =  await MatchSimilarClasses();

            foreach (var similarClass in listOfSimilarClasses)
            {
                var allProperties = GetAllProperties(similarClass);
                var allPropertiesDto = GetAllProperties(similarClass, true);

                var listOfSimilarProperties = new List<MapPropertiesDtoAndDo>();

                var isIgnoreDoProperties = new List<IPropertySymbol>(allProperties.ToList()); // to remove items from it later
                var isIgnoreDtoProperties = new List<IPropertySymbol>(allPropertiesDto.ToList());
                foreach (var codeProperty in allProperties.ToArray())
                {
                    AttributeData attrIgnoreDo = null;
                    if (!string.IsNullOrEmpty(_configuration.MapIgnoreAttribute))
                    {
                        attrIgnoreDo = codeProperty.GetAttributeByName(_configuration.MapIgnoreAttribute);
                    }
                    foreach (var dtoCodeProperty in allPropertiesDto.ToArray())
                    {
                        if (!AreNamesEqual(codeProperty, dtoCodeProperty)) continue;

                        AttributeData attrIgnoreDto = null;

                        if (!string.IsNullOrEmpty(_configuration.MapIgnoreAttribute))
                        {
                            attrIgnoreDto = dtoCodeProperty.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == _configuration.MapIgnoreAttribute);
                        }

                        var attrDo = codeProperty.GetAttributeByName(_configuration.MapAttribute);
                        var attrDto = dtoCodeProperty.GetAttributeByName(_configuration.MapAttribute);

                        var functionFromDto = attrDto?.GetAttributePropertyValue("FunctionFrom");
                        var functionFrom = attrDo?.GetAttributePropertyValue("FunctionFrom");
                        var functionToDto = attrDto?.GetAttributePropertyValue("FunctionTo");
                        var functionTo = attrDo?.GetAttributePropertyValue("FunctionTo");

                        var fromDtoFunction = "";
                        var toDtoFunction = "";

                        if (functionFromDto != null)
                        {
                            if (functionTo != null && functionFromDto != functionTo)
                            {
                                throw new Exception("In the pair: " + similarClass.DOClass.NamedTypeSymbol.Name + " " + similarClass.DtoClass.NamedTypeSymbol.Name +
                                                    " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
                            }

                            fromDtoFunction = functionFromDto;
                        }
                        else
                        {
                            if (functionTo != null)
                            {
                                fromDtoFunction = functionTo;
                            }
                        }

                        if (functionToDto != null)
                        {
                            if (functionFrom != null && functionToDto != functionFrom)
                            {
                                throw new Exception("In the pair: " + similarClass.DOClass.NamedTypeSymbol.Name + " " + similarClass.DtoClass.NamedTypeSymbol.Name +
                                                    " was occured exception. Check pair " + dtoCodeProperty.Name + " " + codeProperty.Name + " properties");
                            }

                            toDtoFunction = functionToDto;
                        }
                        else
                        {
                            if (functionFrom != null)
                            {
                                toDtoFunction = functionFrom;
                            }
                        }

                        if (!CompareTwoPropertyType(codeProperty, dtoCodeProperty, listOfSimilarClasses) && toDtoFunction == "" && fromDtoFunction == "")
                        {
                            var tmp = false;
                            if (IsInNullableDictionary(codeProperty.Type.Name, dtoCodeProperty.Type.Name))
                            {
                                fromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".HasValue ? itemDto." + dtoCodeProperty.Name + ".Value : default(" + codeProperty.Type.Name + ")";
                                toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
                                tmp = true;
                            }

                            if (IsInNullableDictionary(dtoCodeProperty.Type.Name, codeProperty.Type.Name))
                            {
                                toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".HasValue ? item." + codeProperty.Name + ".Value : default(" + dtoCodeProperty.Type.Name + ")";
                                fromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
                                tmp = true;
                            }

                            if (!tmp)
                            {
                                continue;
                            }
                        }

                        var kindOfProperty = GetKindOfMapProperty(codeProperty, listOfSimilarClasses, dtoCodeProperty);
                        var isFlag = false;
                        if (toDtoFunction == "" && fromDtoFunction == "")
                        {
                            switch (kindOfProperty)
                            {
                                case KindOfProperty.AttributeClass:
                                    toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".MapToDto()";
                                    fromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".MapFromDto()";
                                    break;
                                case KindOfProperty.CollectionAttributeClasses:
                                    toDtoFunction = "if(item." + codeProperty.Name + " != null) itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name + ".Select(x => x.MapToDto())";
                                    fromDtoFunction = "if(itemDto." + codeProperty.Name + " != null) item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name + ".Select(x => x.MapFromDto())";
                                    break;
                                case KindOfProperty.EnumToInt:
                                    toDtoFunction = "item." + codeProperty.Name + " = (" + codeProperty.Type.Name + ") itemDto." + dtoCodeProperty.Name;
                                    fromDtoFunction = "itemDto." + dtoCodeProperty.Name + " = (" + dtoCodeProperty.Type.Name + ") item." + codeProperty.Name;
                                    break;
                                case KindOfProperty.IntToEnum:
                                    toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = ("+ dtoCodeProperty.Type.Name + ") item." + codeProperty.Name;
                                    fromDtoFunction = "item." + codeProperty.Name + " = ("+ codeProperty.Type.Name + ") itemDto." + dtoCodeProperty.Name;
                                    break;
                                case KindOfProperty.EnumFlag:
                                    isFlag = true;
                                    toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = item." + codeProperty.Name + ".HasFlag(" + codeProperty.Type.Name + "." + dtoCodeProperty.Name + ")";
                                    fromDtoFunction = "item." + codeProperty.Name + " |= " + "(itemDto." + dtoCodeProperty.Name + " ? " + codeProperty.Type.Name + "." + dtoCodeProperty.Name + " : 0)";
                                    break;
                                default:
                                    toDtoFunction = "itemDto." + dtoCodeProperty.Name + " = " + "item." + codeProperty.Name;
                                    fromDtoFunction = "item." + codeProperty.Name + " = " + "itemDto." + dtoCodeProperty.Name;
                                    break;
                            }
                        }

                        var ignoreBySetter = codeProperty.SetMethod == null || dtoCodeProperty.SetMethod == null;

                        if (attrIgnoreDo == null && attrIgnoreDto == null && !ignoreBySetter)
                        {
                            listOfSimilarProperties.Add(new MapPropertiesDtoAndDo
                            {
                                DOPropertyName = codeProperty,
                                DTOPropertyName = dtoCodeProperty,
                                DOPropertyType = codeProperty.Type,
                                DtoropertyType = dtoCodeProperty.Type,
                                KindOMapfProperty = kindOfProperty,
                                FromDtoFunction = fromDtoFunction,
                                ToDtoFunction = toDtoFunction
                            });
                            isIgnoreDoProperties.Remove(codeProperty);
                            isIgnoreDtoProperties.Remove(dtoCodeProperty);
                        }

                        if (attrIgnoreDto != null)
                        {
                            isIgnoreDtoProperties.Remove(dtoCodeProperty);
                        }

                        allPropertiesDto.Remove(dtoCodeProperty);
                        if (!isFlag)
                        {
                            break;
                        }
                    }
                    if (attrIgnoreDo != null)
                    {
                        isIgnoreDoProperties.Remove(codeProperty);
                    }
                    allProperties.Remove(codeProperty);
                }

                similarClass.MapPropertiesDtoAndDo = listOfSimilarProperties;
                similarClass.IsIgnoreDOProperties = isIgnoreDoProperties.ToArray();
                similarClass.IsIgnoreDTOProperties = isIgnoreDtoProperties.ToArray();
            }

            ListOfSimilarClasses = listOfSimilarClasses;
        }

        private void Validate()
        {
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
        }

        private async Task<List<ClassCompilerInfo>> GetClassesWithMapAttribute(bool dto = false)
        {
            var classesWithMapAttribute = new List<ClassCompilerInfo>();
            var projects = dto ? _configuration.DtoProjects : _configuration.DoProjects;
            foreach (MappingSourceProject project in projects)
            {
                classesWithMapAttribute.AddRange(
                    await _solution.GetAllClasses(project.ProjectName, _configuration.DOSkipAttribute, _configuration.MapAttribute));
            }
            return classesWithMapAttribute;
        }

        private async Task<List<MapDtoAndDo>> MatchSimilarClasses()
        {
            var listOfSimilarClasses = new List<MapDtoAndDo>();
            var classesWithMapAttribute = await GetClassesWithMapAttribute();
            var classesWithMapAttributeDto = await GetClassesWithMapAttribute(true);
            var usedDtos = new HashSet<ClassCompilerInfo>();
            foreach (var doClass in classesWithMapAttribute)
            {
               var isPairFound = false;
                foreach (var dtoClass in classesWithMapAttributeDto)
                {
                    if (GetMapNameForClass(doClass.NamedTypeSymbol) != GetMapNameForClass(dtoClass.NamedTypeSymbol) &&
                        !(_configuration.DOSkipAttribute && GetMapNameForClass(dtoClass.NamedTypeSymbol) == doClass.NamedTypeSymbol.Name) &&
                        !(_configuration.DTOSkipAttribute && GetMapNameForClass(doClass.NamedTypeSymbol) == dtoClass.NamedTypeSymbol.Name)) continue;

                    var doInterface = GetBaseInterface(doClass);
                    var dtoInterface = GetBaseInterface(dtoClass);

                    listOfSimilarClasses.Add(new MapDtoAndDo
                    {
                        DOClass = doClass,
                        DtoClass = dtoClass,
                        DOInterface = doInterface,
                        DtoInterface = dtoInterface
                    });
                    if (isPairFound)
                    {
                        listOfSimilarClasses.Last().IsClassesIntersects = true;
                        foreach (var listOfSimilarClass in listOfSimilarClasses)
                        {
                            if (listOfSimilarClass.DOClass.Equals(doClass))
                            {
                                listOfSimilarClass.IsClassesIntersects = true;
                            }
                        }
                    }
                    if (usedDtos.Contains(dtoClass))
                    {
                        listOfSimilarClasses.Last().IsClassesIntersects = true;
                        foreach (var listOfSimilarClass in listOfSimilarClasses)
                        {
                            if (listOfSimilarClass.DtoClass.Equals( dtoClass))
                            {
                                listOfSimilarClass.IsClassesIntersects = true;
                            }
                        }
                    }

                    usedDtos.Add(dtoClass);
                    isPairFound = true;
                }

                if (!isPairFound)
                {
                    ClassesWithoutPair.Add(doClass);
                }
            }

            foreach (var dtoClass in classesWithMapAttributeDto)
            {
                if (!usedDtos.Contains(dtoClass))
                {
                    ClassesWithoutPair.Add(dtoClass);
                }
            }

            return listOfSimilarClasses;
        }

        private static INamedTypeSymbol GetBaseInterface(ClassCompilerInfo baseClass)
        {
            INamedTypeSymbol baseInterface = null;
            // TODO: recursion for getting base interfaces
            foreach (var @interface in baseClass.NamedTypeSymbol.Interfaces)
            {
                if (@interface.Name.IndexOf("I") == 0 && baseClass.NamedTypeSymbol.Name == @interface.Name.Remove(0, 1))
                {
                    baseInterface = @interface;
                }
            }
            return baseInterface;
        }
        
        private static List<IPropertySymbol> GetAllProperties(MapDtoAndDo similarClass, bool dto = false)
        {
            var allProperties = (dto ?  similarClass.DtoClass : similarClass.DOClass).NamedTypeSymbol.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>().ToList();
            var baseClass = (dto ? similarClass.DtoClass : similarClass.DOClass).NamedTypeSymbol.BaseType;
            while (baseClass != null)
            {
                allProperties.AddRange(baseClass.GetMembers().Where(m => m.Kind == SymbolKind.Property).Cast<IPropertySymbol>());
                baseClass = baseClass.BaseType;
            }

            return allProperties;
        }

        private bool AreNamesEqual(IPropertySymbol codePropertySymbol, IPropertySymbol dtoCodePropertySymbol)
        {
            if (IsPropertyFlagEnum(codePropertySymbol) && GetBaseType(dtoCodePropertySymbol).SpecialType == SpecialType.System_Boolean)
            {
                return IsEnumContainsBoolProperty(codePropertySymbol, dtoCodePropertySymbol);
            }
            return GetMapNameForProperty(codePropertySymbol) == GetMapNameForProperty(dtoCodePropertySymbol);
        }

        private bool IsPropertyFlagEnum(IPropertySymbol codePropertySymbol)
        {
            return GetBaseType(codePropertySymbol).SpecialType == SpecialType.System_Enum && codePropertySymbol.Type.GetAttributes().Any(x => x.AttributeClass.Name == "FlagsAttribute");
        }

        private bool IsEnumContainsBoolProperty(IPropertySymbol @enum, IPropertySymbol property)
        {
            var members = @enum.Type.GetMembers();
            foreach (var member in members)
            {
                if (member.Name == property.Name)
                {
                    return true;
                }
            }

            return false;
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

        public bool CompareTwoPropertyType(IPropertySymbol doCodeProperty, IPropertySymbol dtoCodeProperty, List<MapDtoAndDo> similarClasses)
        {
            var doType = doCodeProperty.Type.GetFullName();
            var dtoType = dtoCodeProperty.Type.GetFullName();

            if (doType.Contains("IEnumerable") && dtoType.Contains("IEnumerable"))
            {
                var tmp1 = doCodeProperty.Type as INamedTypeSymbol;
                doType = tmp1.TypeArguments.First().GetFullName();

                var tmp2 = dtoCodeProperty.Type as INamedTypeSymbol;
                dtoType = tmp2.TypeArguments.First().GetFullName();
            }

            if (doType.Contains("Nullable") && dtoType.Contains("Nullable"))
            {
                var tmp1 = doCodeProperty.Type as INamedTypeSymbol;
                doType = tmp1.TypeArguments.First().GetFullName();

                var tmp2 = dtoCodeProperty.Type as INamedTypeSymbol;
                dtoType = tmp2.TypeArguments.First().GetFullName();
            }

            if (CompareInMapDtoAndDoCollection(doType, dtoType, similarClasses))
            {
                return true;
            }

            if (GetBaseType(doCodeProperty).SpecialType == SpecialType.System_Enum && GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Int32 ||
                GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Enum && GetBaseType(doCodeProperty).SpecialType == SpecialType.System_Int32)
            {
                return true;
            }

            if (IsPropertyFlagEnum(doCodeProperty) && GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Boolean)
            {
                return true;
            }
             
            return doType == dtoType;   
        }

        private static ITypeSymbol GetBaseType(IPropertySymbol property)
        {
            var baseType = property.Type;
            while (baseType.BaseType != null)
            {
                if (baseType.BaseType.SpecialType == SpecialType.System_Object || baseType.BaseType.SpecialType == SpecialType.System_ValueType)
                {
                    break;
                }
                baseType = baseType.BaseType;
            }

            return baseType;
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

        public bool CompareInMapDtoAndDoCollection(string doType, string dtoType, List<MapDtoAndDo> similarClasses)
        {
            var checkingMapClass = similarClasses.FirstOrDefault(x => x.DOClass.NamedTypeSymbol.GetFullName() == doType);
            if (checkingMapClass != null && checkingMapClass == similarClasses.FirstOrDefault(x => x.DtoClass.NamedTypeSymbol.GetFullName() == dtoType))
            {
                return true;
            }

            var similarInterfacesDo = similarClasses.Where(x => x.DOInterface != null).FirstOrDefault(x => x.DOInterface.GetFullName() == doType);
            var similarInterfacesDto = similarClasses.Where(x => x.DtoInterface != null).FirstOrDefault(x => x.DtoInterface.GetFullName() == dtoType);

            if (similarInterfacesDo != null && similarInterfacesDo == similarClasses.FirstOrDefault(x => x.DtoClass.NamedTypeSymbol.GetFullName() == dtoType))
            {
                return true;
            }

            if (similarInterfacesDto != null && similarClasses.FirstOrDefault(x => x.DOClass.NamedTypeSymbol.GetFullName() == doType) == similarInterfacesDto)
            {
                return true;
            }

            if (similarInterfacesDo != null && similarInterfacesDo == similarInterfacesDto)
            {
                return true;
            }

            return false;
        }

        public KindOfProperty GetKindOfMapProperty(IPropertySymbol doCodeProperty, List<MapDtoAndDo> listOfSimilarClasses, IPropertySymbol dtoCodeProperty)
        {
            var type = doCodeProperty.Type.GetFullName();

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

            if (GetBaseType(doCodeProperty).SpecialType == SpecialType.System_Enum && GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Int32)
            {
                return KindOfProperty.EnumToInt;
            }

            if (GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Enum && GetBaseType(doCodeProperty).SpecialType == SpecialType.System_Int32)
            {
                return KindOfProperty.IntToEnum;
            }

            if (IsPropertyFlagEnum(doCodeProperty) && GetBaseType(dtoCodeProperty).SpecialType == SpecialType.System_Boolean)
            {
                return KindOfProperty.EnumFlag;
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
