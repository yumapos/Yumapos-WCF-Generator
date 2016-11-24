using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WCFGenerator.RepositoriesGeneration.Heplers;
using WCFGenerator.RepositoriesGeneration.Services;
using WCFGenerator.SerializeGeneration;
using WCFGenerator.SerializeGeneration.Configuration;
using WCFGenerator.SerializeGeneration.Generation;
using WCFGenerator.SerializeGeneration.Helpers;
using WCFGenerator.SerializeGeneration.Models;

namespace WCFGenerator
{
    public class SerilizationGeneration
    {
        public SerilizationGeneration(string solutionPath)
        {
            _solutionPath = solutionPath;

            var configuration = ConfigurationSettings.GetConfig("serialize") as SerializeConfiguration;
            if (configuration != null)
            {
                _projectNames = configuration.AllProjectNames;
                _generationPrefix = configuration.GenerationPrefix;
                _includeAttribute = configuration.IncludeAtribute;
                _ignoreAttribute = configuration.IgnoreAttribute;
                _baseInterface = configuration.BaseInterface;
                _helpProjects = configuration.AllHelpProjectNames;
                _mappingAttribute = configuration.MappingAttribute;
                _mappingIgnoreAttribute = configuration.MappingIgnoreAttribute;
            }
        }

        private readonly IEnumerable<string> _projectNames;
        private readonly string _solutionPath;

        private static readonly MSBuildWorkspace Workspace = MSBuildWorkspace.Create();
        private static readonly List<Tuple<string, SourceText, string[], string>> Tasks = new List<Tuple<string, SourceText, string[], string>>();

        private Project _project;
        private Solution _solution;

        private readonly string _generationPrefix;

        private readonly string _includeAttribute;

        private readonly string _mappingIgnoreAttribute;

        private readonly IEnumerable<string> _helpProjects; 

        private readonly string _ignoreAttribute;

        private readonly string _baseInterface;
        private readonly string _mappingAttribute;

        private IEnumerable<SourceElements> GetSourceElements(string projectName)
        {
            var findClasses = SyntaxSerilizationHelper.GetAllClasses(projectName, true, "");
            var neededClasses = findClasses.Where(i => i.BaseList != null && i.BaseList.Types.Any(t => t.Type.ToString() == _baseInterface));
            SyntaxSerilizationHelper.InitVisitor(_helpProjects.Union(_projectNames));
            var listElements = new List<SourceElements>();
            foreach (var currentClass in neededClasses)
            {
                if (SyntaxSerilizationHelper.ClassAttributeExist(currentClass, _ignoreAttribute))
                {
                    continue;
                }
                var members = currentClass.Members;
                var generationProperty = new List<PropertyDeclarationSyntax>();
                var generationFields = new List<FieldDeclarationSyntax>();
                var allPublicProperties = new List<PropertyDeclarationSyntax>();
                var exitClass = new SourceElements();
                foreach (var member in members)
                {
                    var field = member as FieldDeclarationSyntax;
                    if (field != null && SyntaxSerilizationHelper.FieldAttributeExist(field, _includeAttribute))
                    {
                        generationFields.Add(field);
                        continue;
                    }

                    var property = member as PropertyDeclarationSyntax;
                    if (property != null)
                    {
                        if (property.Modifiers.Any(x => x.Text == "public"))
                        {
                            allPublicProperties.Add(property);
                        }

                        if (property.AttributeLists.Count == 0 || !SyntaxSerilizationHelper.PropertyAttributeExist(property, _ignoreAttribute))
                        {
                            if (property.Modifiers.Any(x => x.Text == "public"))
                            {
                                generationProperty.Add(property);
                                continue;
                            }
                            if (SyntaxSerilizationHelper.PropertyAttributeExist(property, _includeAttribute))
                            {
                                generationProperty.Add(property);
                            }
                        }
                    }
                }

                if (SyntaxSerilizationHelper.ClassAttributeExist(currentClass, _mappingAttribute))
                {
                    var attr = SyntaxSerilizationHelper.GetAttributesAndPropepertiesCollection(currentClass);
                    var className = attr.FirstOrDefault(x => x.Name == _mappingAttribute)?.GetParameterByKeyName("Name");
                    var findedClass = SyntaxSerilizationHelper.FindClass(className);
                    if (findedClass != null)
                    {
                        var listMappingProperties = new List<PropertyDeclarationSyntax>();
                        foreach (var member in findedClass.Members)
                        {
                            var property = member as PropertyDeclarationSyntax;
                            if (property != null)
                            {
                                if (property.AttributeLists.Count == 0 || !SyntaxSerilizationHelper.PropertyAttributeExist(property, _mappingIgnoreAttribute))
                                {
                                    if (property.Modifiers.Any(x => x.Text == "public"))
                                    {
                                        listMappingProperties.Add(property);
                                    }
                                }
                            }
                        }
                        exitClass.MappingClass = findedClass;
                        exitClass.MappingProperties = listMappingProperties;
                    }
                }
                exitClass.AllPublicProperties = allPublicProperties;
                exitClass.CurrentClass = currentClass;
                exitClass.Properties = generationProperty;
                exitClass.Fields = generationFields;
                listElements.Add(exitClass);
            }
            return listElements;
        }

        private Tuple<string, string> GetNamespace(ClassDeclarationSyntax currentClass)
        {
            var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(currentClass.SyntaxTree);

            var model = compilation.GetSemanticModel(currentClass.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(currentClass);

            var baseClass = FindBaseStatefulClass(symbol.BaseType);

            var fullName = symbol.ToString();
            var indexClassName = fullName.LastIndexOf('.');
            var nameSpace = fullName.Substring(0, indexClassName); 
            return new Tuple<string, string>(nameSpace, baseClass);
        }

        private string FindBaseStatefulClass(INamedTypeSymbol baseClass)
        {
            var findedClass = SyntaxSerilizationHelper.FindClass(baseClass.Name);
            if (findedClass != null)
            {
                var isImplementStatefulObject = findedClass.BaseList.Types.Any(t => t.Type.ToString() == _baseInterface);
                if (isImplementStatefulObject)
                {
                    return baseClass.Name+_generationPrefix;
                }
            }

            return string.Empty;
        }

        private IEnumerable<GenerationElements> GetGenerationElements(IEnumerable<SourceElements> sourceElements)
        {
            var listGenEl = new List<GenerationElements>();
            SyntaxSerilizationHelper.InitVisitor(_projectNames);
            var namespaces = new List<string>();
            foreach (var sourceElem in sourceElements)
            {
                var textElements = new GenerationElements();
                var nameSpaceAndBaseClass = GetNamespace(sourceElem.CurrentClass);
                var propList = new List<GenerationProperty>();
                foreach (var property in sourceElem.Properties)
                {
                    propList.Add(new GenerationProperty
                    {
                        Name = EditingSerializationHelper.GetPropertyName(property.Identifier.Text),
                        Type = property.Type.ToFullString(),
                        VariableClassName = property.Identifier.Text,
                        IsSetterExist = property.SetterExist(),
                    });
                }

                foreach (var field in sourceElem.Fields)
                {
                    propList.Add(new GenerationProperty
                    {
                        Name = EditingSerializationHelper.GetPropertyName(field.Declaration.Variables.First().Identifier.ValueText),
                        Type = field.Declaration.Type.ToFullString(),
                        VariableClassName = field.Declaration.Variables.First().Identifier.ValueText,
                        IsSetterExist = true
                    });
                }

                textElements.IsPropertyEquals = false;

                if (sourceElem.MappingClass != null)
                {
                    var mapClassName = sourceElem.MappingClass.Identifier.Text;
                    var mappingProperies = new List<GenerationProperty>();
                    foreach (var property in sourceElem.MappingProperties)
                    {
                        var mapProperty = sourceElem.AllPublicProperties.FirstOrDefault(x => x.Identifier.Text.Trim() == property.Identifier.Text.Trim());
                        if (mapProperty == null)
                        {
                            continue;
                        }
                        mappingProperies.Add(new GenerationProperty
                        {
                            Name = EditingSerializationHelper.GetPropertyName(property.Identifier.Text),
                            Type = property.Type.ToFullString(),
                            VariableClassName = property.Identifier.Text,
                            IsSetterExist = property.SetterExist()
                        });
                    }
                    textElements.MapClassName = mapClassName;
                    textElements.MapProperties = mappingProperies;
                    textElements.IsPropertyEquals = TextSerializationHelper.IsCollectionEquals(mappingProperies, propList);
                    namespaces.Add(GetNamespace(sourceElem.MappingClass).Item1);
                }

                textElements.ClassName = sourceElem.CurrentClass.Identifier.Text;
                textElements.Properties = propList;
                textElements.GeneratedClassName = $"{sourceElem.CurrentClass.Identifier.Text}{_generationPrefix}";
                textElements.Namespace = nameSpaceAndBaseClass.Item1;
                textElements.UsingNamespaces = namespaces.Union(TextSerializationHelper.GetUsingNamespaces(sourceElem.CurrentClass));
                textElements.ClassAccessModificator = sourceElem.CurrentClass.Modifiers.FirstOrDefault().ValueText;
                textElements.SerializableBaseClassName = nameSpaceAndBaseClass.Item2;
                listGenEl.Add(textElements);
            }
            return listGenEl;
        }

        public async void GenerateAll()
        {
            _solution = await Workspace.OpenSolutionAsync(_solutionPath);
            SyntaxSerilizationHelper.Solution = _solution;
            if (_projectNames != null)
            {
                foreach (var st in _projectNames)
                {
                    _project = _solution.Projects.First(x => x.Name == st);

                    SyntaxSerilizationHelper.Project = _project;

                    var allElements = GetSourceElements(st);
                    var structElements = GetGenerationElements(allElements);

                    foreach (var generatedClass in structElements)
                    {
                        var patternText = new TextSerializationPatterns(generatedClass);
                        var fullGenClass = patternText.GeneratePartialClass();
                        CreateDocument(fullGenClass.ToString(), st, "Extensions/" + generatedClass.ClassName + ".g.cs");
                    }
                    ApplyChanges();
                }
            }

            Workspace.TryApplyChanges(_solution);
            Workspace.CloseSolution();
        }

        private static void CreateDocument(string code, string projectName, string fileName)
        {
            if (fileName != null && projectName != null)
            {
                code = "//The file " + fileName + " was automatically generated using WCF-Generator.exe\r\n\r\n\r\n" + code;

                var folders = fileName.Split('/');
                fileName = folders[folders.Length - 1];
                folders = folders.Where((val, idx) => idx != folders.Length - 1).ToArray();

                var sourceText = SourceText.From(code);
                Tasks.Add(Tuple.Create(fileName, sourceText, folders, projectName));
            }
        }

        private void ApplyChanges()
        {
            foreach (var doc in Tasks)
            {
                _project = _solution?.Projects.FirstOrDefault(x => x.Name == doc.Item4);

                var oldDocument = _project?.Documents.FirstOrDefault(x => x.Name == doc.Item1);

                if (oldDocument != null)
                {
                    if (oldDocument.GetTextAsync().Result.ToString() != doc.Item2.ToString())
                    {
                        bool isEqual = oldDocument.Folders.SequenceEqual(doc.Item3);

                        if (isEqual)
                        {
                            _project = _project.RemoveDocument(oldDocument.Id);
                        }

                        var newDocument = _project?.AddDocument(doc.Item1, doc.Item2, doc.Item3);
                        _project = newDocument?.Project;
                        _solution = _project?.Solution;
                    }
                }
                else
                {
                    var newDocument = _project?.AddDocument(doc.Item1, doc.Item2, doc.Item3);
                    _project = newDocument?.Project;
                    _solution = _project?.Solution;
                }
            }
        }
    }
}
