using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Helpers;

namespace WCFGenerator.RepositoriesGeneration.Analysis
{
    internal class SolutionSyntaxWalker 
    {
        private static readonly SymbolDisplayFormat FullQualifiedDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        private readonly CSharpCompilation _fullCompilation;
        private readonly CSharpCompilation _croppedCompilation;

        // Classes from all projects with repository models marked repository attribute
        private readonly List<ClassDeclarationSyntax> _repositoryModelClasses;

        // Classes from target projects
        private readonly List<ClassDeclarationSyntax> _customRepositoryClasses;

        // interfaces from all projects with repository interfaces
        private readonly List<InterfaceDeclarationSyntax> _repositoryInterfaces;

        public SolutionSyntaxWalker(Solution solution, List<string> repositoryModelsProjects, string repositoryAttributeName, List<string> repositoryInterfaceProjects, string targetProject,  List<string> additionalProjectsForAnalysis)
        {
            if (solution == null) throw new ArgumentException("solution");
            if (repositoryModelsProjects == null) throw new ArgumentException("repositoryModelsProjects");
            if (repositoryAttributeName == null) throw new ArgumentException("repositoryAttributeName");
            if (repositoryInterfaceProjects == null) throw new ArgumentException("repositoryInterfaceProjects");
            if (targetProject == null) throw new ArgumentException("targetProject");

            var treesByProject = LoadTreesByProjectAsync(solution).Result;
            var allTrees = treesByProject.SelectMany(g => g).ToList();

            var selectedProjects = repositoryInterfaceProjects
                .Concat(repositoryInterfaceProjects)
                .Concat(additionalProjectsForAnalysis)
                .Distinct()
                .ToList();

            var selectedTrees = treesByProject.Where(x => selectedProjects.Contains(x.Key))
                .SelectMany(x => x).ToList();
            
            _croppedCompilation = CSharpCompilation.Create("CroppedCompilation")
                .AddSyntaxTrees(selectedTrees)
                .WithReferences(new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location)
                });
            
            _fullCompilation = CSharpCompilation.Create("FullCompilation")
                .AddSyntaxTrees(allTrees)
                .WithReferences(new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location)
                });
    
            _repositoryModelClasses = treesByProject
                .Where(g => repositoryModelsProjects.Contains(g.Key))
                .SelectMany(g => g)
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .Where(c => c.AttributeExist(repositoryAttributeName))
                .ToList();
    
            _customRepositoryClasses = treesByProject[targetProject]
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .ToList();
    
            _repositoryInterfaces = treesByProject
                .Where(g => repositoryInterfaceProjects.Contains(g.Key))
                .SelectMany(g => g)
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                .ToList();
        }

        public IEnumerable<ClassDeclarationSyntax> GetRepositoryClasses()
        {
            return _repositoryModelClasses;
        }

        /// <summary>
        ///     Search custom repository class by name. Skip genereted Classes.
        /// </summary>
        public IEnumerable<ClassDeclarationSyntax> FindCustomRepositoryClassByName(string className)
        {
            var resultList = _customRepositoryClasses.Where(c => c.Identifier.Text == className);

            return resultList;
        }

        /// <summary>
        ///     Search type name
        /// </summary>
        public string GetFullTypeName(string typeName)
        {
            var resultList = _croppedCompilation.GetSymbolsWithName(s => s == typeName);
            return resultList.FirstOrDefault()?.ToString();
        }

        public InterfaceDeclarationSyntax GetInheritedInterface(string interfaceName)
        {
            var resultList = _repositoryInterfaces.FirstOrDefault(i => i.BaseList !=null && i.BaseList.Types.Any(t => t.Type.ToString() == interfaceName));
            return resultList;
        }

        public string GetFullRepositoryModelName(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace + "." + codeclass.Identifier.Text;
        }

        public string GetCustomRepositoryNamespace(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace.ToString();
        }
        public string GetTypeNamespace(BaseTypeDeclarationSyntax codeclass)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(codeclass.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(codeclass);

            return symbol.ContainingNamespace.ToString();
        }

        public string GetFullPropertyTypeName(PropertyDeclarationSyntax prop)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(prop.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(prop);
            
            return symbol.GetMethod.ReturnType.ToString();
        }

        public (string FullyQualifiedName, bool Nullable) GetFullPropertyTypeNameAndNullable(PropertyDeclarationSyntax prop)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(prop.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(prop);
            var typeSymbol = symbol.Type;
            var nullable = false;
            if (typeSymbol is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                nullable = true;
                typeSymbol = namedType.TypeArguments.FirstOrDefault();
            }

            return (typeSymbol.ToDisplayString(FullQualifiedDisplayFormat), nullable);
        }
        
        
        public Type GetReturnType(TypeSyntax returnType)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(returnType.SyntaxTree);

            var symbol = semanticModel.GetTypeInfo(returnType);

            return symbol.Type.GetType();
        }

        public bool IsBaseClass(BaseTypeSyntax syntax)
        {
            return _repositoryModelClasses.Any(c => c.Identifier.Text == syntax.Type.ToString());
        }

        public bool PropertyIsEnum(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(propertyDeclarationSyntax.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(propertyDeclarationSyntax);
            var typeSymbol = symbol.Type;
            
            if (typeSymbol is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                var nullableArg = namedType.TypeArguments.FirstOrDefault();
                if (nullableArg != null && nullableArg.TypeKind == TypeKind.Enum)
                {
                    return true;
                }
            }

            return typeSymbol.TypeKind == TypeKind.Enum;
        }
        public string GetEnumUnderlyingTypeFullName(PropertyDeclarationSyntax prop)
        {
            var semanticModel = _fullCompilation.GetSemanticModel(prop.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(prop);
            if (symbol == null) return "System.Int32";
    
            var typeSymbol = symbol.GetMethod?.ReturnType;
            if (typeSymbol == null) return "System.Int32";
            

            if (typeSymbol is INamedTypeSymbol namedType && 
                namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                var nullableArg = namedType.TypeArguments.FirstOrDefault();
                if (nullableArg != null && nullableArg.TypeKind == TypeKind.Enum)
                {
                    var enumSymbol = (INamedTypeSymbol)nullableArg;
                    var underlying = enumSymbol.EnumUnderlyingType;
                    return underlying?.ToString() ?? "System.Int32";
                }
                return "System.Int32";
            }
    
            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                var enumtype = (INamedTypeSymbol)typeSymbol;
                var underlying = enumtype.EnumUnderlyingType;
                return underlying?.ToDisplayString(FullQualifiedDisplayFormat) ?? "System.Int32";
            }
    
            return null;
        }
        public string GetAttributeArgumentValue(ClassDeclarationSyntax parentClass, string propertyName, string attributeName, string argumentName)
        {
            return GetAttributeArgumentValueFromSyntax(parentClass, propertyName, attributeName, argumentName);

            // TODO Restore, research why NamedArguments list is empty
            //return GetAttributeArgumentValueFromCompilationFromNamedArguments(codeclass, propertyName, attributeName, argumentName);
        }

        
        private async Task<ILookup<string, SyntaxTree>> LoadTreesByProjectAsync(Solution solution)
        {
            var projects = solution.Projects.ToList();
            var tasks = projects.Select(async project =>
            {
                var trees = await GetTrees(solution, new List<string> { project.Name });
                return new { ProjectName = project.Name, Trees = trees };
            });
            var results = await Task.WhenAll(tasks);
            return results
                .SelectMany(r => r.Trees, (r, tree) => new { r.ProjectName, Tree = tree })
                .ToLookup(x => x.ProjectName, x => x.Tree);
        }
        
        private static async Task<List<SyntaxTree>> GetTrees(
            Solution solution,
            List<string> projects,
            LanguageVersion targetVersion = LanguageVersion.Latest)
        {
            var uniqueProjectNames = projects.Distinct().ToList();
            var targetProjects = solution.Projects.Where(p => uniqueProjectNames.Contains(p.Name)).ToList();
            var trees = new List<SyntaxTree>();
            foreach (var project in targetProjects)
            {
                foreach (var document in project.Documents)
                {
                    if (document.Name.Contains(".g.cs"))
                        continue;

                    var filePath = document.FilePath;
                    if (string.IsNullOrEmpty(filePath))
                        continue;

                    var tree = await document.GetSyntaxTreeAsync();
                    if (tree == null) continue;

                    var parseOptions = tree.Options as CSharpParseOptions;
                    if (parseOptions != null && parseOptions.LanguageVersion != targetVersion)
                    {
                        var newParseOptions = parseOptions.WithLanguageVersion(targetVersion);
                        tree = tree.WithRootAndOptions(await tree.GetRootAsync(), newParseOptions);
                    }
                    trees.Add(tree);
                }
            }

            return trees;
        }
        
        private static string GetAttributeArgumentValueFromSyntax(ClassDeclarationSyntax parentClass, string propertyName, string attributeName, string argumentName)
        {
            var prop = parentClass.Members.OfType<PropertyDeclarationSyntax>().First(cp =>
                cp.GetterExist() && cp.SetterExist() && cp.Identifier.Text == propertyName);

            var ignoreAtt = SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(prop)
                .FirstOrDefault(x => x.Name.ToString().Contains(attributeName));
            
            var value = ignoreAtt?.Parameters?.FirstOrDefault(pair => pair.Key.Contains(argumentName));

            return value?.Value;
        }

        private string GetAttributeArgumentValueFromCompilationFromNamedArguments(BaseTypeDeclarationSyntax codeclass,
            string propertyName, string attributeName, string argumentName)
        {
            var symbol = _fullCompilation.GetClass(codeclass);

            if (symbol == null)
            {
                return null;
            }

            var property = symbol.GetMembers()
                .Where(m => m.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>().FirstOrDefault(m => m.Name == propertyName);

            if (property == null)
            {
                return null;
            }

            var attribute = property.GetAttributeByName(attributeName);

            if (attribute == null)
            {
                return null;
            }

            var value = attribute.GetAttributePropertyValue(argumentName);

            return value;
        }
    }
}
