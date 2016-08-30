using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionedRepositoryGeneration.Generator.Analysis;
using VersionedRepositoryGeneration.Generator.Core;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Services
{
    internal class RepositoryService
    {
        #region Private

        private readonly Solution _solution;
        private readonly GeneratorConfiguration _config;
        private readonly Project _project;

        #endregion

        #region Constructor

        public RepositoryService(RepositoryGeneratorWorkSpace workSpace, GeneratorConfiguration config)
        {
            _solution = workSpace.Solution;
            _project = workSpace.Project;
            _config = config;
        }

        #endregion

        #region Find versioned repositories

        public IEnumerable<ICodeClassGeneratorRepository> GetRepositories()
        {
            var listOfSimilarClasses = new List<BaseCodeClassGeneratorRepository>();

            foreach (var repositoryClasses in _config.RepositoryClassProjects)
            {
                // Get all clases marked RepositoryAttribute
                var findClasses = SolutionSyntaxWalker.GetAllClassesWhithAttribute(_solution, repositoryClasses, false, RepositoryDataModelHelper.DataAccessAttributeName).Result;
                var versionedRepositories = findClasses.SelectMany(c=> GetAllRepositories(c)).Where(c=> c!=null);

                listOfSimilarClasses.AddRange(versionedRepositories);
            }

            // Apply info from base clasess
            foreach (var r in listOfSimilarClasses.Where(c=> c.RepositoryAnalysisError == null))
            {
                var repositoryInfo = r.RepositoryInfo;
                
                // Find base class
                if (repositoryInfo.DOClass.BaseList != null)
                {
                    var baseClassType = repositoryInfo.DOClass.BaseList.Types.FirstOrDefault();
                    var baseClass = GetClassByName(baseClassType.ToString());

                    if (baseClass != null)
                    {
                        var baseSimilarClass = listOfSimilarClasses.FirstOrDefault(x => x.RepositoryInfo.ClassName == baseClass.Identifier.Text);
                    
                        if (baseSimilarClass != null && baseSimilarClass.RepositoryAnalysisError == null)
                        {
                            repositoryInfo.PrimaryKeys.AddRange(baseSimilarClass.RepositoryInfo.PrimaryKeys);
                            repositoryInfo.MethodImplementationInfo.ToList().AddRange(baseSimilarClass.RepositoryInfo.MethodImplementationInfo);

                            repositoryInfo.JoinedClass = baseSimilarClass.RepositoryInfo.DOClass;
                            repositoryInfo.JoinedClassName = baseSimilarClass.RepositoryInfo.DOClass.Identifier.ToString();
                            repositoryInfo.JoinedElements = baseSimilarClass.RepositoryInfo.Elements;
                            //repositoryInfo.PrimaryKeyJoined = baseSimilarClass.RepositoryInfo.Keys[0];
                            repositoryInfo.TableNameJoined = baseSimilarClass.RepositoryInfo.TableName;
                            // TODO что тут за хрень
                            repositoryInfo.IsIdentityJoined = TenantRelationExist(baseSimilarClass.RepositoryInfo.DOClass);
                            repositoryInfo.FilterDataJoined = baseSimilarClass.RepositoryInfo.FilterData;
                        }
                    }
                }
            }

            return listOfSimilarClasses;
        }

        #endregion

        #region Private

        private IEnumerable<BaseCodeClassGeneratorRepository> GetAllRepositories(ClassDeclarationSyntax doClass)
        {
            var list = new List<BaseCodeClassGeneratorRepository>();

            // Get DataAccess attribute from repository model
            var dataAccessAttr = GetAttributesAndPropepertiesCollection(doClass)
                .FirstOrDefault(x => x.Name == RepositoryDataModelHelper.DataAccessAttributeName || x.Name + "Attribute" == RepositoryDataModelHelper.DataAccessAttributeName);
            if (dataAccessAttr == null)
            {
                list.Add(CreateWithAnalysisError(doClass.Identifier.Text, _config.RepositorySuffix, "Repository model not marked DataAccess attribute."));
                return list;
            }
            
            // common repository info
            var repositoryInfo = new RepositoryInfo()
            {
                DOClass = doClass,
                RepositorySuffix = _config.RepositorySuffix,
                GenericRepositoryName = string.Format("I{0}<{1}>", _config.RepositorySuffix, doClass.Identifier.Text),
                ClassName = doClass.Identifier.Text,
                ClassFullName = SyntaxAnalysisHelper.GetNameSpaceByClass(doClass, _project) + "." + doClass.Identifier,
                IsTenantRelated = TenantRelationExist(doClass),
            };

            #region Repository interface

            // Get implemented interfaces for custom repository (for example - IMenuItemRepository)

            var repositoryInterfaces = SolutionSyntaxWalker.GetImplementedInterfaces(_solution, _config.RepositoryInterfacesProjectName, repositoryInfo.GenericRepositoryName).Result;

            var repoInterface = repositoryInterfaces.FirstOrDefault(r => r.Identifier.Text != null
                                                                         && r.Identifier.Text.IndexOf("I", StringComparison.Ordinal) == 0
                                                                         && r.Identifier.Text.Remove(0, 1) == doClass.Identifier + _config.RepositorySuffix);
            if (repoInterface == null)
            {
                list.Add(CreateWithAnalysisError(doClass.Identifier.Text, _config.RepositorySuffix, "Class not implemented true named repository interface."));
                return list;
            }

            #endregion

            #region Repository model attributes

            #region DataAccess

            var dataAccess = AttributeService.GetDataAccessAttribute(dataAccessAttr);

            var tableName = dataAccess.TableName ?? doClass.Identifier.ToString() + 's';
            repositoryInfo.TableName = tableName;

            // Get filters
            var filterKeys = (new[] {dataAccess.FilterKey1, dataAccess.FilterKey2, dataAccess.FilterKey3})
                .Where(fk => fk != null)
                .Select(fk =>
                {
                    // Filter data may be combined (ItemId, GroupId)
                    var filters = fk.Split(',').Select(f=>
                    {
                        var parameter = doClass.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(cp => (GetterInCodePropertyExist(cp) && SetterInCodePropertyExist(cp) && cp.Identifier.Text == f));
                        return new ParameterInfo(fk, parameter != null ? parameter.Type.ToFullString() : null);
                    }).ToList();
                    return new FilterInfo(string.Join("And", fk.Split(',')), filters);
                });

            // Common filter - isDeleted
            if (dataAccess.IsDeleted)
            {
                repositoryInfo.SpecialOptions = new FilterInfo("IsDeleted", new List<ParameterInfo> { new ParameterInfo("IsDeleted", typeof(bool).FullName, "false") });
            }

            repositoryInfo.FilterInfos.AddRange(filterKeys);

            var isVersioning = dataAccess.TableVersion != null;
            repositoryInfo.IsVersioning = isVersioning;

            if (isVersioning)
            {
                // repository service
                var repository = new CodeClassGeneratorVersionedRepositoryService()
                {
                    RepositoryInfo = repositoryInfo
                };
                list.Add(repository);

                // version repository
                var versionsRepository = new CodeClassGeneratorVersionsRepository()
                {
                    RepositoryInfo = repositoryInfo
                };
                list.Add(versionsRepository);

                // cache repository
                var cacheRepository = new CodeClassGeneratorСacheRepository()
                {
                    RepositoryInfo = repositoryInfo
                };
                list.Add(cacheRepository);
            }
            else
            {
                // base repository 
                var repositoryAndDo = new CodeClassGeneratorRepository
                {
                    RepositoryInfo = repositoryInfo
                };
                list.Add(repositoryAndDo);
            }

            #endregion

            #endregion

            #region Property attributes

            // Get only properties
            foreach (var prop in doClass.Members.OfType<PropertyDeclarationSyntax>().Where(cp => (GetterInCodePropertyExist(cp) && SetterInCodePropertyExist(cp))))
            {
                // Add sql column name
                repositoryInfo.Elements.Add(prop.Identifier.Text);

                // Skip property without attributes
                if (prop.AttributeLists.Count <= 0) continue;

                var attributes = prop.AttributeLists.First();

                foreach (var attr in attributes.Attributes)
                {
                    if (attr.Name.ToString() == RepositoryDataModelHelper.KeyAttributeName)
                    {
                        repositoryInfo.PrimaryKeys.Add(new ParameterInfo(prop.Identifier.Text, prop.Type.ToFullString()));
                    }
                    else if (attr.Name.ToString() == RepositoryDataModelHelper.VesionKeyAttributeName)
                    {
                        repositoryInfo.VersionKey = prop.Identifier.Text;
                    }
                    else if (attr.Name.ToString() == RepositoryDataModelHelper.DbIgnoreAttributeName)
                    {
                        repositoryInfo.Elements.RemoveAt(repositoryInfo.Elements.Count - 1);
                    }
                    else if (attr.Name.ToString() == RepositoryDataModelHelper.DataMany2ManyAttributeName)
                    {
                        // TODO ManyToMany
                    }
                }
            }

            #endregion

            // Repository interface info
            repositoryInfo.RepositoryInterface = repoInterface;
            repositoryInfo.RepositoryInterfaceMethods = SolutionSyntaxWalker.GetMethodsFromMembers(repoInterface.Members.ToList());
            
            // Check exist custom repository class
            var customRepository = GetClassByName(doClass.Identifier + _config.RepositorySuffix);
            var customRepoExist = customRepository != null;

            string nameSpace;

            // Get custom repository class info
            if (customRepoExist)
            {
                // Custom repository info
                repositoryInfo.CustomRepository = customRepository;
                repositoryInfo.CustomRepositoryMethods = SolutionSyntaxWalker.GetMethodsFromMembers(customRepository.Members.ToList());
                // Check constructor exist
                repositoryInfo.IsConstructorImplemented = ConstructorImplementationExist(customRepository);
                
                // Get repositories NameSpace
                var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(customRepository.SyntaxTree);
                var model = compilation.GetSemanticModel(customRepository.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(customRepository);
                var fullName = symbol.ToString();
                nameSpace = fullName.Replace("." + customRepository.Identifier.Text, "");
            }
            else
            {
                nameSpace = _config.RepositoryMainPlace;
            }

            repositoryInfo.RepositoriesNamespace = nameSpace;

            // Search method for implementation
            repositoryInfo.MethodImplementationInfo = GetUnimplementedMethods(repositoryInfo);
            
            return list;
        }
        
        private static bool SetterInCodePropertyExist(PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.SetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
        private static bool GetterInCodePropertyExist(PropertyDeclarationSyntax codeproperty)
        {
            try
            {
                if (codeproperty.AccessorList.Accessors.FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration) != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static IEnumerable<AttributeAndPropeperties> GetAttributesAndPropepertiesCollection(MemberDeclarationSyntax element)
        {
            SyntaxList<AttributeListSyntax> attributes = new SyntaxList<AttributeListSyntax>();

            var codeClass = element as ClassDeclarationSyntax;
            if (codeClass != null)
                attributes = codeClass.AttributeLists;

            var codeProperty = element as PropertyDeclarationSyntax;
            if (codeProperty != null)
                attributes = codeProperty.AttributeLists;

            var attributeCollection = new List<AttributeAndPropeperties>();
            var listOfStringProperties = new List<string>();

            //            Regex attributesRegex = new Regex(@"\[\s*(?<Name>\w*)\s*(\((?<arguments>.*)\))?\s*\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //            MatchCollection matchesAttrs = attributesRegex.Matches(attributeString);

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

        private bool TenantRelationExist(ClassDeclarationSyntax doClass)
        {
            var interfaces = SolutionSyntaxWalker.GetImplementedInterfaces(_solution, _config.RepositoryMainPlace, doClass).Result;

            return interfaces.All(inter => inter.Identifier.ToString().Trim() != "ITenantUnrelated");
        }

        private static bool ConstructorImplementationExist(ClassDeclarationSyntax customRepository)
        {
            return customRepository.Members.Any(e => (e as ConstructorDeclarationSyntax) != null);
        }

        private static IEnumerable<MethodImplementationInfo> GetUnimplementedMethods(RepositoryInfo info)
        {
            IEnumerable<string> unimplemented = new List<string>();

            if (info.RepositoryInterfaceMethods != null)
            {
                // Check implemented methods in custom repository
                var temp = info.CustomRepositoryMethods != null
                    ? info.RepositoryInterfaceMethods.Where(im => info.CustomRepositoryMethods.FirstOrDefault(cm => cm.Identifier.Text == im.Identifier.Text) == null)
                    : info.RepositoryInterfaceMethods;
                unimplemented = temp.Select(m => m.Identifier.Text);
            }

            // Set methods which possible to generate
            var allmethods = SelectPossibleImplementation(unimplemented, info);
            return allmethods;
        }

        private static IEnumerable<MethodImplementationInfo> SelectPossibleImplementation(IEnumerable<string> unimplementedMethods, RepositoryInfo info)
        {
            // Add common methods
            var methods = new List<MethodImplementationInfo>
            {
                new MethodImplementationInfo() { Method = RepositoryMethod.GetAll, RequiresImplementation = false},
                new MethodImplementationInfo() { Method = RepositoryMethod.Insert, Parameters = new List<ParameterInfo>() {new ParameterInfo(info.ParameterName, info.ClassFullName, TODO)}, RequiresImplementation = false},
            };

            // Methods by keys from model (without methods from base model)
            var possibleKeyMethods = info.GetPossibleKeysForMethods();

            // TODO Как это будет выглядеть? GetAllRecipieItem?
            //if (joinName != null)
            //{
            //    methods.AddRange(new List<MethodImplementationInfo>()
            //    {
            //        new MethodImplementationInfo() {Method = RepositoryMethod.GetAll, JoinName = joinName, RequiresImplementation = false},
            //        new MethodImplementationInfo() {Method = RepositoryMethod.Insert, JoinName = joinName, RequiresImplementation = false},
            //    });
            //}

            // Add methods based on keys
            foreach (var method in possibleKeyMethods)
            {
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.GetBy, Key = method.Key, Parameters = method.Parameters, RequiresImplementation = false });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.UpdateBy, Key = method.Key, Parameters = method.Parameters, RequiresImplementation = false });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.RemoveBy, Key = method.Key, Parameters = method.Parameters, RequiresImplementation = false });

                //if (joinName == null) continue;

                //methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.GetBy, Key = name, JoinName = joinName, RequiresImplementation = false });
                //methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.UpdateBy, Key = name, JoinName = joinName, RequiresImplementation = false });
                //methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.RemoveBy, Key = name, JoinName = joinName, RequiresImplementation = false });
            }

            // Set methods to implementation from possible list
            foreach (var um in unimplementedMethods)
            {
                // Seach in possible list
                var m = methods.FirstOrDefault(o => o.Method.GetName() + (o.Key ?? "") == um);
                if (m == null) continue;
                // Set to implementation
                m.RequiresImplementation = true;
            }

            return methods;
        }

        private static BaseCodeClassGeneratorRepository CreateWithAnalysisError(string doClassName, string repositorySuffix, string message)
        {
            return new CodeClassGeneratorRepository()
            {
                RepositoryInfo = new RepositoryInfo()
                {
                    ClassName = doClassName,
                    RepositorySuffix = repositorySuffix
                },
                RepositoryAnalysisError = message
            };

        }

        private ClassDeclarationSyntax GetClassByName(string name)
        {
            return SolutionSyntaxWalker.FindClassByName(_solution, _config.ProjectName, name, _config.RepositoryTargetFolder).Result;
        }

        #endregion
    }
}
