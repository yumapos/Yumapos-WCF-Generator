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
using WCFGenerator.SerializeGeneration;

namespace WCFGenerator
{
    public class SerilizationGeneration
    {
        public SerilizationGeneration(string solutionPath)
        {
            _solutionPath = solutionPath;

            var configuration = ConfigurationSettings.GetConfig("serialize") as SerializeConfiguration;
            _projectNames = configuration.AllProjectNames;
            _generationPrefix = configuration.GenerationPrefix;
            _includeAttribute = configuration.IncludeAtribute;
            _ignoreAttribute = configuration.IgnoreAttribute;
            _baseInterface = configuration.BaseInterface;
        }

        private IEnumerable<string> _projectNames;
        private string _solutionPath;

        private static readonly MSBuildWorkspace _workspace = MSBuildWorkspace.Create();
        private static List<Tuple<string, SourceText, string[], string>> _tasks = new List<Tuple<string, SourceText, string[], string>>();

        Project _project;
        Solution _solution;

        private string _generationPrefix;

        private string _includeAttribute;

        private string _ignoreAttribute;

        private string _baseInterface;

        private IEnumerable<SourceElements> GetSourceElements(string projectName)
        {
            var findClasses = GeneratorHelper.GetAllClasses(projectName, true, "").Result;
            var neededClasses = findClasses.Where(i => i.BaseList != null && i.BaseList.Types.Any(t => t.Type.ToString() == _baseInterface));
            var listElements = new List<SourceElements>();
            foreach (var currentClass in neededClasses)
            {
                var members = currentClass.Members;
                var generationProperty = new List<PropertyDeclarationSyntax>();
                var generationFields = new List<FieldDeclarationSyntax>();
                foreach (var member in members)
                {
                    var field = member as FieldDeclarationSyntax;
                    if (field != null && FieldAttributeExist(field, _includeAttribute))
                    {
                        generationFields.Add(field);
                        continue;
                    }

                    var property = member as PropertyDeclarationSyntax;
                    if (property != null)
                    {
                        if (property.AttributeLists.Count == 0 || !PropertyAttributeExist(property, _ignoreAttribute))
                        {
                            if (property.Modifiers.Any(x => x.Text == "public"))
                            {
                                generationProperty.Add(property);
                                continue;
                            }
                            if (PropertyAttributeExist(property, _includeAttribute))
                            {
                                generationProperty.Add(property);
                                continue;
                            }
                        }
                    }
                }
                listElements.Add(new SourceElements
                {
                    CurrentClass = currentClass,
                    Properties = generationProperty,
                    Fields = generationFields
                });
            }
            return listElements;
        }

        private string GetNamespace(ClassDeclarationSyntax currentClass)
        {
            var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(currentClass.SyntaxTree);

            var model = compilation.GetSemanticModel(currentClass.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(currentClass);

            var fullName = symbol.ToString();
            var indexClassName = fullName.LastIndexOf('.');
            var nameSpace = fullName.Substring(0, indexClassName); 
            return nameSpace;
        }

        private IEnumerable<GenerationElements> GetGenerationElements(IEnumerable<SourceElements> sourceElements)
        {
            var listGenEl = new List<GenerationElements>();
            foreach (var sourceElem in sourceElements)
            {
                var nameSpace = GetNamespace(sourceElem.CurrentClass);
                var propList = new List<GenerationProperty>();
                foreach (var property in sourceElem.Properties)
                {
                    propList.Add(new GenerationProperty
                    {
                        Name = GetPropertyName(property.Identifier.Text),
                        Type = property.Type.ToFullString(),
                        VariableClassName = property.Identifier.Text
                    });
                }

                foreach (var field in sourceElem.Fields)
                {
                    propList.Add(new GenerationProperty
                    {
                        Name = GetPropertyName(field.Declaration.Variables.First().Identifier.ValueText),
                        Type = field.Declaration.Type.ToFullString(),
                        VariableClassName = field.Declaration.Variables.First().Identifier.ValueText
                    });
                }

                listGenEl.Add(new GenerationElements
                {
                    ClassName = sourceElem.CurrentClass.Identifier.Text,
                    Properties = propList,
                    GeneratedClassName = $"{sourceElem.CurrentClass.Identifier.Text}{_generationPrefix}",
                    Namespace = nameSpace,
                    UsingNamespaces = GetUsingNamespaces(sourceElem.CurrentClass),
                    ClassAccessModificator = sourceElem.CurrentClass.Modifiers.FirstOrDefault().ValueText
                });
            }
            return listGenEl;
        }

        private StringBuilder GenerateInterface(GenerationElements elements)
        {
            var exitInterface = new StringBuilder();
            exitInterface.AppendFormat("\t internal partial class {0}", elements.GeneratedClassName);
            exitInterface.Append("\r\n");
            exitInterface.Append("\t {");
            exitInterface.Append("\r\n");
            foreach (var member in elements.Properties)
            {
                exitInterface.Append("\t\t public ");
                exitInterface.Append(member.Type);
                exitInterface.Append(" ");
                exitInterface.Append(member.Name);
                exitInterface.Append(" { get; set;}");
                exitInterface.Append("\r\n");
            }

            exitInterface.Append("\t }");
            exitInterface.Append("\r\n");

            return exitInterface;
        }

        private StringBuilder GeneratePartialClass(GenerationElements elements)
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

            foreach (var namesp in elements.UsingNamespaces)
            {
                generation.Append("using ");
                generation.Append(namesp);
                generation.Append("; \r\n");
            }
            generation.Append("\r\n");
            generation.Append("namespace ");
            generation.Append(elements.Namespace);
            generation.Append("\r\n{");
            generation.Append("\r\n");

            generation.Append(GenerateInterface(elements));

            generation.AppendFormat("\t {0} partial class {1}",elements.ClassAccessModificator, elements.ClassName);
            generation.Append("\r\n");
            generation.Append("\t {");
            generation.Append("\r\n");
            var variableName = FirstSymbolToLower(elements.GeneratedClassName);
            //Generate virtual method for SetState
            generation.AppendFormat("\t\t partial void StateCustomizationOnSet({0} {1});", elements.GeneratedClassName, variableName);
            generation.Append("\r\n");
            generation.Append("\r\n");

            //Generate virtual method for GetState
            generation.AppendFormat("\t\t partial void StateCustomizationOnGet({0} {1});", elements.GeneratedClassName, variableName);
            generation.Append("\r\n");
            generation.Append("\r\n");

            //Generate Set state
            generation.Append("\t\t public void SetState(object value)");
            generation.Append("\r\n");
            generation.Append("\t\t {");
            generation.Append("\r\n");
            generation.AppendFormat("\t\t\t var core = value as {0};", elements.GeneratedClassName);
            generation.Append("\r\n");

            foreach (var prop in elements.Properties)
            {
                generation.AppendFormat("\t\t\t {1} = core.{0};", prop.Name, prop.VariableClassName);
                generation.Append("\r\n");
            }
            generation.Append("\t\t\t StateCustomizationOnSet(core);");
            generation.Append("\r\n");
            generation.Append("\t\t }");
            generation.Append("\r\n");
            generation.Append("\r\n");

            //Generate Get state
            generation.Append("\t\t public object GetState()");
            generation.Append("\r\n");
            generation.Append("\t\t {");
            generation.Append("\r\n");
            generation.AppendFormat("\t\t\t var core = new {0}();", elements.GeneratedClassName);
            generation.Append("\r\n");
            foreach (var prop in elements.Properties)
            {
                generation.AppendFormat("\t\t\t core.{0} = {1};", prop.Name, prop.VariableClassName);
                generation.Append("\r\n");
            }
            generation.Append("\t\t\t StateCustomizationOnGet(core);");
            generation.Append("\r\n");
            generation.Append("\t\t\t return core;");
            generation.Append("\r\n");
            generation.Append("\t\t }");
            generation.Append("\r\n");
            generation.Append("\t }");
            generation.Append("\r\n");
            generation.Append("}");

            return generation;
        }

        private string FirstSymbolToLower(string parms)
        {
            if (string.IsNullOrEmpty(parms))
            {
                return string.Empty;
            }
            return char.ToLower(parms[0]) + parms.Substring(1);
        }

        private string FirstSymbolToUpper(string parms)
        {
            if (string.IsNullOrEmpty(parms))
            {
                return string.Empty;
            }
            return char.ToUpper(parms[0]) + parms.Substring(1);
        }

        private string DeleteFirstSymbol(string parms)
        {
            parms = parms.Substring(1);
            return parms;
        }

        private string GetPropertyName(string variableName)
        {
            if (variableName.StartsWith("_"))
            {
                variableName = DeleteFirstSymbol(variableName);
            }
            if (char.IsLower(variableName[0]))
            {
                variableName = FirstSymbolToUpper(variableName);
            }
            return variableName;
        }

        public async void GenerateAll()
        {
            _solution = await _workspace.OpenSolutionAsync(_solutionPath);
            GeneratorHelper.Solution = _solution;
            foreach (var st in _projectNames)
            {
                _project = _solution.Projects.First(x => x.Name == st);

                GeneratorHelper.Project = _project;

                var allElements = GetSourceElements(st);
                var structElements = GetGenerationElements(allElements);

                foreach (var generatedClass in structElements)
                {
                    var fullGenClass = GeneratePartialClass(generatedClass);
                    CreateDocument(fullGenClass.ToString(), st, "Extensions/" + generatedClass.ClassName + ".g.cs");
                }
                ApplyChanges(st);
            }
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
                _tasks.Add(Tuple.Create(fileName, sourceText, folders, projectName));
            }
        }

        private void ApplyChanges(string projectChange)
        {
            foreach (var doc in _tasks)
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

            _workspace.TryApplyChanges(_project.Solution);
        }

        private static bool PropertyAttributeExist(PropertyDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        private static bool FieldAttributeExist(FieldDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        private IEnumerable<string> GetUsingNamespaces(ClassDeclarationSyntax classes)
        {
            var syntaxTree = classes.SyntaxTree;
            var root = (CompilationUnitSyntax)syntaxTree.GetRoot();


            return root.Usings.Select(x => x.Name.ToString());
        }

        private class SourceElements
        {
            public ClassDeclarationSyntax CurrentClass { get; set; }
            public List<PropertyDeclarationSyntax> Properties { get; set; }
            public List<FieldDeclarationSyntax> Fields { get; set; }
        }

        private class GenerationElements
        {
            public string ClassName { get; set; }

            public string GeneratedClassName { get; set; }

            public List<GenerationProperty> Properties { get; set; }

            public List<GenerationProperty> Fields { get; set; }

            public string Namespace { get; set; }

            public IEnumerable<string> UsingNamespaces { get; set; }

            public string ClassAccessModificator {get; set;}
        }

        private class GenerationProperty
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string VariableClassName { get; set; }
        }
    }
}
