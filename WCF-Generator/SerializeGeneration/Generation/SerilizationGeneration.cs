using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using WCFGenerator.Common;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.SerializeGeneration.Configuration;
using WCFGenerator.SerializeGeneration.Helpers;
using WCFGenerator.SerializeGeneration.Models;

namespace WCFGenerator.SerializeGeneration.Generation
{
    public class SerilizationGeneration : IGeneration
    {
        private readonly GeneratorWorkspace _generatorWorkspace;
        private readonly TextMigrationPatterns _textMigrationPatterns;

        public SerilizationGeneration(GeneratorWorkspace generatorWorkspace)
        {
            _generatorWorkspace = generatorWorkspace;

            var configuration = SerializeGeneratorSettings.Current.GetConfig();
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
                _migrationProject = configuration.MigrationProject;
                _migrationVersionClass = configuration.MigrationVersionClass;
                _migrationInterface = configuration.MigrationInterface;
                _migrationClassPrefix = configuration.MigrationClassPrefix;
                _migrationIgnoreAttribute = configuration.MigrationIgnoreAttribute;
                _migrationVersionProject = configuration.MigrationVersionProject;
            }
            _textMigrationPatterns = new TextMigrationPatterns();
            
        }

        private readonly IEnumerable<string> _projectNames;

        private static readonly List<Tuple<string, SourceText, string[], string>> Tasks = new List<Tuple<string, SourceText, string[], string>>();

        private Project _project;

        private readonly string _generationPrefix;

        private readonly string _includeAttribute;

        private readonly string _mappingIgnoreAttribute;

        private readonly IEnumerable<string> _helpProjects; 

        private readonly string _ignoreAttribute;

        private readonly string _baseInterface;
        private readonly string _mappingAttribute;

        private readonly string _migrationProject;
        private readonly string _migrationVersionClass;
        private readonly string _migrationInterface;
        private readonly string _migrationClassPrefix;
        private readonly string _migrationIgnoreAttribute;
        private readonly string _migrationVersionProject;

        private IEnumerable<SourceElements> GetSourceElements(string projectName)
        {
            var findClasses = SyntaxSerilizationHelper.GetAllClasses(projectName, true, skipGeneratedClasses:false).ToArray();
            var neededClasses = findClasses.Where(i => i.BaseList != null && i.Identifier.Text != _baseInterface.TrimStart('I') && i.BaseList.Types.Any(t => t.Type.ToString() == _baseInterface));
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
                        var propertyType = property.Type.ToFullString().Replace(" ", "");

                        if (property.Modifiers.Any(x => x.Text == "public"))
                        {
                            allPublicProperties.Add(property);
                        }

                        // Properties with ignore attribute should not be serialised
                        if (property.AttributeLists.Count != 0 && SyntaxSerilizationHelper.PropertyAttributeExist(property, _ignoreAttribute))
                        {
                            continue;
                        }

                        // All public properties should be serialised 
                        if (property.Modifiers.Any(x => x.Text == "public"))
                        {
                            // Check invalid type for serialisation
                            ThrowIfIvalidPropertyType(propertyType, currentClass, property);
                            generationProperty.Add(property);
                        }
                        else if(SyntaxSerilizationHelper.PropertyAttributeExist(property, _includeAttribute))
                        {
                            // Check invalid type for serialisation
                            ThrowIfIvalidPropertyType(propertyType, currentClass, property);
                            generationProperty.Add(property);
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
                exitClass.LastGenerationProperties = GetPropertyPreviousGeneratedClass(currentClass.Identifier.Value.ToString(), findClasses);
                exitClass.AllPublicProperties = allPublicProperties;
                exitClass.CurrentClass = currentClass;
                exitClass.Properties = generationProperty;
                exitClass.Fields = generationFields;
                listElements.Add(exitClass);
            }
            return listElements;
        }

        private void ThrowIfIvalidPropertyType(string propertyType, ClassDeclarationSyntax currentClass,
            PropertyDeclarationSyntax property)
        {
// TODO Temp solution. Need use compilation for find and compare types
            if (SerializeGeneratorSettings.Current.InvalidPropertyTypes.Any(t => propertyType.Contains(t)))
            {
                var error =
                    $"Property {currentClass.Identifier.Text}.{property.Identifier.Text} have not allowed type \"{propertyType}\". " +
                    $"Attributes: {string.Join("", property.AttributeLists.SelectMany(a => a.Attributes).Select(a => a.Name))}";
                this.ThrowAnalyzingException(error);
            }
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
            var namespaces = new List<string>();
            foreach (var sourceElem in sourceElements)
            {
                var textElements = new GenerationElements();
                var nameSpaceAndBaseClass = GetNamespace(sourceElem.CurrentClass);

                var propList = sourceElem.Properties.Select(property => new GenerationProperty
                {
                    Name = EditingSerializationHelper.GetPropertyName(property.Identifier.Text),
                    Type = property.Type.ToFullString().Replace(" ",""),
                    VariableClassName = property.Identifier.Text,
                    IsSetterExist = property.SetterExist(),
                }).ToList();

                propList.AddRange(sourceElem.Fields.Select(field => new GenerationProperty
                {
                    Name = EditingSerializationHelper.GetPropertyName(field.Declaration.Variables.First().Identifier.ValueText),
                    Type = field.Declaration.Type.ToFullString().Replace(" ", ""),
                    VariableClassName = field.Declaration.Variables.First().Identifier.ValueText,
                    IsSetterExist = true
                }));

                var previousGenerationProperties = sourceElem.LastGenerationProperties.Select(property => new GenerationProperty
                {
                    Name = EditingSerializationHelper.GetPropertyName(property.Identifier.Text),
                    Type = property.Type.ToFullString().Replace(" ", ""),
                    VariableClassName = property.Identifier.Text,
                    IsSetterExist = property.SetterExist()
                }).ToList();

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
                textElements.ChangeWithPreviousProperties = CollectionComparator.Compare(propList, previousGenerationProperties,(s, x) => s.Name == x.Name,
                    (s, x) => string.Equals(s.Type,"PosMoney", StringComparison.InvariantCulture) || string.Equals(x.Type,s.Type, StringComparison.InvariantCulture));
                listGenEl.Add(textElements);
            }
            return listGenEl;
        }

        public void GenerateAll()
        {
            SyntaxSerilizationHelper.Solution = _generatorWorkspace.Solution;
            var migrationElements = new List<MigrationElements>();
            if (_projectNames != null)
            {
                SyntaxSerilizationHelper.InitVisitor(_helpProjects.Union(_projectNames));
                foreach (var projectName in _projectNames)
                {
                    _project = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == projectName);

                    if(_project == null) this.ThrowAnalyzingException($"Project {projectName} not found");

                    SyntaxSerilizationHelper.Project = _project;

                    var allElements = GetSourceElements(projectName);
                    var structElements = GetGenerationElements(allElements);
                    
                    foreach (var generatedClass in structElements)
                    {
                        if (generatedClass.ChangeWithPreviousProperties.WasChanged)
                        {
                            migrationElements.Add(new MigrationElements
                            {
                                ClassNameWithNameSpace = generatedClass.Namespace+"."+generatedClass.ClassName,
                                ChangeWithPreviousProperties = generatedClass.ChangeWithPreviousProperties
                            });
                        }
                        var patternText = new TextSerializationPatterns(generatedClass);
                        var fullGenClass = patternText.GeneratePartialClass();
                        CreateDocument(fullGenClass.ToString(), projectName, "Extensions/" + generatedClass.ClassName + ".g.cs");
                    }
                    ApplyChanges();
                }
            }
            if (migrationElements.Any())
            {
                var newVersion = GenerateNewVersion();
                GenerateMigration(newVersion, migrationElements);
            }

            _generatorWorkspace.ApplyChanges();
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
                _project = _generatorWorkspace.Solution?.Projects.FirstOrDefault(x => x.Name == doc.Item4);

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
                        _generatorWorkspace.Solution = _project?.Solution;
                    }
                }
                else
                {
                    var newDocument = _project?.AddDocument(doc.Item1, doc.Item2, doc.Item3);
                    _project = newDocument?.Project;
                    _generatorWorkspace.Solution = _project?.Solution;
                }
            }
        }

        private List<PropertyDeclarationSyntax> GetPropertyPreviousGeneratedClass(string currentClassName, IEnumerable<ClassDeclarationSyntax> allProjectClasses)
        {
            var previousGenerationClass = allProjectClasses.Where(x => x.Identifier.Value.ToString() == currentClassName + _generationPrefix).ToList();
            if (previousGenerationClass.Any())
            {
                var properties = Enumerable.Empty<PropertyDeclarationSyntax>();
                foreach (var classDeclarationSyntax in previousGenerationClass)
                {
                    properties = properties.Union(classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>().Where(x => !SyntaxSerilizationHelper.PropertyAttributeExist(x, _migrationIgnoreAttribute)));
                }
                return properties.ToList();
            }
            return new List<PropertyDeclarationSyntax>();
        }

        private int GetCurrentVersion()
        {
            var migrationProject = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == _migrationVersionProject);
            if (migrationProject == null)
            {
                throw new Exception("Project for migration not found");
            }
            SyntaxSerilizationHelper.InitVisitor(new[] { _migrationVersionProject });
            SyntaxSerilizationHelper.Project = migrationProject;
            var targetClass = SyntaxSerilizationHelper.FindClass(_migrationVersionClass);
            if (targetClass == null)
            {
                return 0;
            }
            var versionField = targetClass.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(property => property.Identifier.ValueText == "CurrentVersion");
            return int.Parse(versionField.ExpressionBody.Expression.GetFirstToken().ValueText);
        }

        private int GenerateNewVersion()
        {
            var version = GetCurrentVersion() + 1;
            var versionClass = _textMigrationPatterns.GenerateVersionNumberClass(version, _migrationVersionClass, _migrationVersionProject);
            CreateDocument(versionClass, _migrationVersionProject, "Helpers/Generation/" +_migrationVersionClass + ".cs");
            ApplyChanges();

            return version;
        }

        private void GenerateMigration(int version, List<MigrationElements> migrationElements)
        {
            var migrationProject = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == _migrationProject);
            SyntaxSerilizationHelper.Project = migrationProject;
            var migration = _textMigrationPatterns.GenerateNewMigrationClass(_migrationProject, _migrationClassPrefix, _migrationInterface,
                version, migrationElements);
            CreateDocument(migration, _migrationProject, "GeneratedMigrations/" + $"{_migrationClassPrefix}V{version}" + ".g.cs");
            ApplyChanges();
        }
    }
}
