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
using WCFGenerator.RepositoriesGeneration.Services;
using WCFGenerator.SerializeGeneration;
using WCFGenerator.SerializeGeneration.Configuration;

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
            _helpProjects = configuration.AllHelpProjectNames;
            _mappingAttribute = configuration.MappingAttribute;
            _mappingIgnoreAttribute = configuration.MappingIgnoreAttribute;
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
            var listElements = new List<SourceElements>();
            foreach (var currentClass in neededClasses)
            {
                if (ClassAttributeExist(currentClass, _ignoreAttribute))
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
                    if (field != null && FieldAttributeExist(field, _includeAttribute))
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
                            }
                        }
                    }
                }

                if (ClassAttributeExist(currentClass, _mappingAttribute))
                {
                    var attr = GetAttributesAndPropepertiesCollection(currentClass);
                    var className = attr.FirstOrDefault(x => x.Name == _mappingAttribute)?.GetParameterByKeyName("Name");
                    var findedClass = SyntaxSerilizationHelper.FindClass(className, _helpProjects);
                    if (findedClass != null)
                    {
                        var listMappingProperties = new List<PropertyDeclarationSyntax>();
                        foreach (var member in findedClass.Members)
                        {
                            var property = member as PropertyDeclarationSyntax;
                            if (property != null)
                            {
                                if (property.AttributeLists.Count == 0 || !PropertyAttributeExist(property, _mappingIgnoreAttribute))
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
            var namespaces = new List<string>();
            foreach (var sourceElem in sourceElements)
            {
                var textElements = new GenerationElements();
                var nameSpace = GetNamespace(sourceElem.CurrentClass);
                var propList = new List<GenerationProperty>();
                foreach (var property in sourceElem.Properties)
                {
                    propList.Add(new GenerationProperty
                    {
                        Name = GetPropertyName(property.Identifier.Text),
                        Type = property.Type.ToFullString(),
                        VariableClassName = property.Identifier.Text,
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
                            Name = GetPropertyName(property.Identifier.Text),
                            Type = property.Type.ToFullString(),
                            VariableClassName = property.Identifier.Text,
                        });
                    }
                    textElements.MapClassName = mapClassName;
                    textElements.MapProperties = mappingProperies;
                    textElements.IsPropertyEquals = IsCollectionEquals(mappingProperies, propList);
                    namespaces.Add(GetNamespace(sourceElem.MappingClass));
                }

                textElements.ClassName = sourceElem.CurrentClass.Identifier.Text;
                textElements.Properties = propList;
                textElements.GeneratedClassName = $"{sourceElem.CurrentClass.Identifier.Text}{_generationPrefix}";
                textElements.Namespace = nameSpace;
                textElements.UsingNamespaces = namespaces.Union(GetUsingNamespaces(sourceElem.CurrentClass));
                textElements.ClassAccessModificator = sourceElem.CurrentClass.Modifiers.FirstOrDefault().ValueText;
                listGenEl.Add(textElements);
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
                exitInterface.Append(member.Type.Trim() == "PosMoney" ? "decimal" : member.Type);
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

            var className = elements.GeneratedClassName;
            if (elements.IsPropertyEquals)
            {
                className = elements.MapClassName;
            }
            else
            {
                generation.Append(GenerateInterface(elements));
            }

            generation.AppendFormat("\t {0} partial class {1}", elements.ClassAccessModificator, elements.ClassName);
            generation.Append("\r\n");
            generation.Append("\t {");
            generation.Append("\r\n");
            
            generation.Append(GenerateMethodsOrMappings(elements.Properties, className, true));
            if (elements.MapClassName != null && elements.MapProperties != null)
            {
                generation.Append("\r\n");
                generation.Append(GenerateMethodsOrMappings(elements.MapProperties, elements.MapClassName, false));
            }

            generation.Append("\r\n");
            generation.Append("\t }");
            generation.Append("\r\n");
            generation.Append("}");

            return generation;
        }

        private StringBuilder GenerateMethodsOrMappings(List<GenerationProperty> mainProp, string className, bool isSerialization)
        {
            var variableName = FirstSymbolToLower(className);
            var stringBuilder = new StringBuilder();
            
            var functionPrefix = isSerialization ? "Do" : "BDo";
            var getMethodsName = isSerialization ? "GetDataObject" : $"Get{className}Do";
            var setMethodsName = isSerialization ? "SetDataObject" : $"Set{className}Do";
            //Generate private property
            if (isSerialization)
            {
                stringBuilder.AppendFormat("\t\t private {0} _boDo;", className);
                stringBuilder.Append("\r\n");
            }
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
            stringBuilder.AppendFormat("\t\t public void {0}(object value{1})", setMethodsName,
                isSerialization ? ", YumaPos.FrontEnd.Infrastructure.Persistence.IDeserializationContext context" : "");
            
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t {");
            stringBuilder.Append("\r\n");
            stringBuilder.AppendFormat("\t\t\t var dataObject = value as {0};", className);
            stringBuilder.Append("\r\n");

            foreach (var prop in mainProp)
            {
                if (prop.Type.Trim() == "PosMoney")
                {
                    stringBuilder.AppendFormat("\t\t\t {0} = new PosMoney(context.MonetarySettings);", prop.VariableClassName);
                    stringBuilder.Append("\r\n");
                    stringBuilder.AppendFormat("\t\t\t  {0}.Value = dataObject.{1};", prop.VariableClassName,prop.Name);
                    stringBuilder.Append("\r\n");
                }
                else
                {
                    stringBuilder.AppendFormat("\t\t\t {1} = dataObject.{0};", prop.Name, prop.VariableClassName);
                    stringBuilder.Append("\r\n");
                }
            }
            stringBuilder.AppendFormat("\t\t\t {0}CustomizationOnSet(dataObject{1});",functionPrefix,
                isSerialization ? ",context" : "");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t }");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\r\n");

            //Generate Get state
            stringBuilder.AppendFormat("\t\t public object {0}()", getMethodsName);
            stringBuilder.Append("\r\n");
            stringBuilder.Append("\t\t {");
            stringBuilder.Append("\r\n");
            if (isSerialization)
            {
                stringBuilder.Append("\t\t\t if (_boDo == null)");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t {");
                stringBuilder.Append("\r\n");
                stringBuilder.AppendFormat("\t\t\t\t _boDo = new {0}();",className);
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
                    stringBuilder.AppendFormat("\t\t\t\t {2}.{0} = {1}.Value;", prop.Name, prop.VariableClassName,
                        isSerialization ? "_boDo" : "dataObject");
                    stringBuilder.Append("\r\n");
                }
                else
                {
                    stringBuilder.AppendFormat("\t\t\t\t {2}.{0} = {1};", prop.Name, prop.VariableClassName,
                        isSerialization ? "_boDo" : "dataObject");
                    stringBuilder.Append("\r\n");
                }
            }
            stringBuilder.AppendFormat("\t\t\t\t {0}CustomizationOnGet(ref {1});", functionPrefix, 
                isSerialization ? "_boDo" : "dataObject");
            stringBuilder.Append("\r\n");
            if (isSerialization)
            {
                stringBuilder.Append("\t\t\t }");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t return _boDo;");
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
                stringBuilder.Append("\t\t public void ResetDataObject()");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t {");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t\t _boDo = null;");
                stringBuilder.Append("\r\n");
                stringBuilder.Append("\t\t }");
            }

            return stringBuilder;
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
            _solution = await Workspace.OpenSolutionAsync(_solutionPath);
            SyntaxSerilizationHelper.Solution = _solution;
            foreach (var st in _projectNames)
            {
                _project = _solution.Projects.First(x => x.Name == st);

                SyntaxSerilizationHelper.Project = _project;

                var allElements = GetSourceElements(st);
                var structElements = GetGenerationElements(allElements);

                foreach (var generatedClass in structElements)
                {
                    var fullGenClass = GeneratePartialClass(generatedClass);
                    CreateDocument(fullGenClass.ToString(), st, "Extensions/" + generatedClass.ClassName + ".g.cs");
                }
                ApplyChanges();
            }
            Workspace.TryApplyChanges(_solution);
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

        private bool IsCollectionEquals(List<GenerationProperty> firstCollection, List<GenerationProperty> secondCollection)
        {
            if (firstCollection.Count != secondCollection.Count)
                return false;
            foreach (var generationProperty in firstCollection)
            {
                var prop = secondCollection.FirstOrDefault(x => x.Name == generationProperty.Name);
                if (prop == null || prop.Type.Trim() != generationProperty.Type.Trim())
                {
                    return false;
                }
            }
            return true;
        } 

        private static bool PropertyAttributeExist(BasePropertyDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        private bool FieldAttributeExist(FieldDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }

        private bool ClassAttributeExist(ClassDeclarationSyntax p, string attributeName)
        {
            return p.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == attributeName));
        }


        public List<AttributeAndPropeperties> GetAttributesAndPropepertiesCollection(ClassDeclarationSyntax element)
        {
            SyntaxList<AttributeListSyntax> attributes = new SyntaxList<AttributeListSyntax>();

            var codeClass = element;
            if (codeClass != null)
                attributes = codeClass.AttributeLists;

            var attributeCollection = new List<AttributeAndPropeperties>();
            var listOfStringProperties = new List<string>();

            foreach (var ca in attributes)
            {
                foreach (var attr in ca.Attributes)
                {
                    var properties = attr.ArgumentList?.ToString() ?? "";

                    var dictionaryOfAttributes = new Dictionary<string, string>();
                    var countProperties = 0;
                    listOfStringProperties.Clear();

                    Regex attributesRegex = new Regex(@"(@""(?:""""|[^""])*"")|(""(?:\\""|\\r|\\n|\\t|\\\\|[^""\\])*"")",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    MatchCollection matchesProperties = attributesRegex.Matches(properties);

                    foreach (var property in matchesProperties)
                    {
                        properties = properties.Replace(property.ToString(), "%%string" + countProperties + "%%");
                        listOfStringProperties.Add(property.ToString());
                        countProperties++;
                    }

                    countProperties = 0;
                    foreach (string prop in properties.Split(',').ToList())
                    {
                        var property = prop.Replace("(", "").Replace(")", "");
                        if (property.Contains("%%string"))
                        {
                            if (property.Split(':', '=').Count() == 2)
                            {
                                if (property.Split(':', '=')[1].Contains("%%string"))
                                    dictionaryOfAttributes.Add(property.Split(':', '=')[0],
                                        listOfStringProperties[
                                            Convert.ToInt32(property.Split(':', '=')[1].Replace("%%string", "")
                                                .Replace("%", ""))].Replace("\"", ""));
                            }
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(),
                                    listOfStringProperties[
                                        Convert.ToInt32(property.Replace("%%string", "").Replace("%", ""))].Replace("\"", ""));
                        }
                        else
                        {
                            if (property.Split(':', '=').Count() == 2)
                                dictionaryOfAttributes.Add(property.Split(':', '=')[0], property.Split(':', '=')[1]);
                            else
                                dictionaryOfAttributes.Add(countProperties.ToString(), property);
                        }

                        countProperties++;
                    }

                    attributeCollection.Add(new AttributeAndPropeperties
                    {
                        Name = attr.Name.ToString(),
                        Parameters = dictionaryOfAttributes
                    });

                }
            }

            return attributeCollection;
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
            public List<PropertyDeclarationSyntax> AllPublicProperties { get; set; }
            public List<FieldDeclarationSyntax> Fields { get; set; }
            public ClassDeclarationSyntax MappingClass { get; set; }
            public List<PropertyDeclarationSyntax> MappingProperties { get; set; } 
        }

        private class GenerationElements
        {
            public string ClassName { get; set; }

            public string GeneratedClassName { get; set; }

            public List<GenerationProperty> Properties { get; set; }

            public string MapClassName { get; set; }

            public List<GenerationProperty> MapProperties { get; set; }

            public List<GenerationProperty> AllPublicProperties { get; set; }

            public string Namespace { get; set; }

            public IEnumerable<string> UsingNamespaces { get; set; }

            public string ClassAccessModificator {get; set;}

            public bool IsPropertyEquals { get; set; }
            
        }

        private class GenerationProperty
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string VariableClassName { get; set; }
        }
    }
}
