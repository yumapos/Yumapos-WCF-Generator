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

        IDictionary<string, CSharpCompilation> _doProjectsCompilations = new Dictionary<string, CSharpCompilation>();

        public IList<ClassCompilerInfo> ClassesWithoutPair = new List<ClassCompilerInfo>();

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
            var isPairFounded = false;
            while (classesWithMapAttribute.Any())
            {
                isPairFounded = false;
                var doClass = classesWithMapAttribute.First();
                foreach (var dtoClass in classesWithMapAttributeDto)
                {
                    INamedTypeSymbol doInterface = null;
                    INamedTypeSymbol dtoInterface = null;

                    if (GetMapName(doClass.NamedTypeSymbol, true) == GetMapName(dtoClass.NamedTypeSymbol, true) ||
                        (_configuration.DOSkipAttribute && GetMapName(dtoClass.NamedTypeSymbol, true) == doClass.NamedTypeSymbol.Name) ||
                        (_configuration.DTOSkipAttribute && GetMapName(doClass.NamedTypeSymbol, true) == dtoClass.NamedTypeSymbol.Name))
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
                        isPairFounded = true;
                        break;
                    }
                }

                if (!isPairFounded)
                {
                    ClassesWithoutPair.Add(doClass);
                }
                classesWithMapAttribute.Remove(doClass);
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
            }
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

        public string GetMapName(INamedTypeSymbol element, bool isClass)
        {
            string value = element.Name;
            const string nameProperty = "Name";

            var attributes = element.GetAttributes();

            foreach (var ca in attributes)
            {
                if (ca.AttributeClass.Name.Contains(_configuration.MapAttribute) && ca.NamedArguments.Any(a => a.Key.Contains(nameProperty)))
                {
                    if (!String.IsNullOrEmpty(_configuration.DoSuffix) && isClass)
                    {
                        if (value.EndsWith(_configuration.DoSuffix.ToLower()))
                        {
                            value = value.Replace(_configuration.DoSuffix.ToLower(), "");
                        }
                    }

                    if (!String.IsNullOrEmpty(_configuration.DtoSuffix) && isClass)
                    {
                        if (value.EndsWith(_configuration.DtoSuffix.ToLower()))
                        {
                            value = value.Replace(_configuration.DtoSuffix.ToLower(), "");
                        }
                    }
                }
            }

            value = value.ToLower();

            if (!String.IsNullOrEmpty(_configuration.DoSuffix) && isClass)
            {
                if (value.EndsWith(_configuration.DoSuffix.ToLower()))
                {
                    value = value.Replace(_configuration.DoSuffix.ToLower(), "");
                }
            }

            if (!String.IsNullOrEmpty(_configuration.DtoSuffix) && isClass)
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

        // TODO: use fully qualified name here to distinguish similar-named classes https://stackoverflow.com/a/23308759
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

    }
}
