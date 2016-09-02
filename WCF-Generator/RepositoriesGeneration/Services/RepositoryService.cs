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
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;

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
                if (repositoryInfo.BaseClassName != null)
                {
                    var baseClass = SolutionSyntaxWalker.FindClassByName(_solution, _config.ProjectName, repositoryInfo.BaseClassName, _config.RepositoryTargetFolder).Result;

                    if (baseClass != null)
                    {
                        var baseSimilarClass = listOfSimilarClasses.FirstOrDefault(x => x.RepositoryInfo.ClassName == repositoryInfo.BaseClassName);
                    
                        if (baseSimilarClass != null && baseSimilarClass.RepositoryAnalysisError == null)
                        {
                            baseSimilarClass.RepositoryInfo.IsJoned = true;
                            repositoryInfo.JoinRepositoryInfo = baseSimilarClass.RepositoryInfo;
                        }
                    }
                }
            }

            // Return without attached (it can not generate)
            var resultRepositories = listOfSimilarClasses.Where(r => r.RepositoryInfo.IsJoned == false).ToList();

            // Fill method implementation info for main repositories
            foreach (var repo in resultRepositories.Where(r => !r.RepositoryName.Contains(CodeClassGeneratorVersionsRepository.RepositoryKind) && !r.RepositoryName.Contains(CodeClassGeneratorCacheRepository.RepositoryKind)))
            {
                var methods = GetUnimplementedMethods(repo.RepositoryInfo);
                repo.RepositoryInfo.MethodImplementationInfo.AddRange(methods);
            }

            var many2ManyInfos = resultRepositories.Where(r => r.RepositoryName.Contains(CodeClassGeneratorVersionsRepository.RepositoryKind)).SelectMany(repository => repository.RepositoryInfo.Many2ManyInfo);

            foreach (var many2Many in many2ManyInfos)
            {
                // get namespaces for generate reference in repository
                many2Many.RepositoryNamespaces = resultRepositories
                    .Where(r => (r.RepositoryName.StartsWith(many2Many.EntityType) || r.RepositoryName.StartsWith(many2Many.ManyToManyEntytyType)) && r.RepositoryName.EndsWith(r.RepositoryInfo.RepositorySuffix) && r.RepositoryInfo.RepositoryNamespace != null)
                    .Select(r => r.RepositoryInfo.RepositoryNamespace).Distinct().ToList();

                // get repository info by  EntityType from [many2manyAttribute]
                many2Many.EntityRepositoryInfo = resultRepositories
                    .Where(r => r.RepositoryName.StartsWith(many2Many.EntityType) && r.RepositoryName.EndsWith(r.RepositoryInfo.RepositorySuffix))
                    .Select(r => r.RepositoryInfo)
                    .FirstOrDefault();

                // get repository info by ManyToManyEntytyType from [many2manyAttribute]
                many2Many.ManyToManyRepositoryInfo = resultRepositories
                    .Where(r => r.RepositoryName.StartsWith(many2Many.ManyToManyEntytyType) && r.RepositoryName.EndsWith(r.RepositoryInfo.RepositorySuffix))
                    .Select(r => r.RepositoryInfo)
                    .FirstOrDefault();
            }

            return resultRepositories;
        }

        #endregion

        #region Analysis methods

        private IEnumerable<BaseCodeClassGeneratorRepository> GetAllRepositories(ClassDeclarationSyntax doClass)
        {
            var list = new List<BaseCodeClassGeneratorRepository>();
            var className = doClass.Identifier.Text;

            // Get DataAccess attribute from repository model
            var dataAccessAttr = SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(doClass).FirstOrDefault(x => x.Name.ToString().Contains(RepositoryDataModelHelper.DataAccessAttributeName));

            if (dataAccessAttr == null)
            {
                list.Add(CreateWithAnalysisError(className, _config.RepositorySuffix, "Repository model not marked DataAccess attribute."));
                return list;
            }

            // common repository info
            var repositoryInfo = new RepositoryInfo()
            {
                RepositorySuffix = _config.RepositorySuffix,
                ClassName = className,
                ClassFullName = SyntaxAnalysisHelper.GetNameSpaceByClass(doClass, _project) + "." + doClass.Identifier,
            };

            // Base class name
            if (doClass.BaseList != null && doClass.BaseList.Types.Any())
            {
                repositoryInfo.BaseClassName = doClass.BaseList.Types.FirstOrDefault().ToString();
            }

            #region Repository interface

            // Get implemented interfaces for custom repository (for example - IMenuItemRepository)

            var repositoryInterfaces = SolutionSyntaxWalker.GetImplementedInterfaces(_solution, _config.RepositoryInterfacesProjectName, repositoryInfo.GenericRepositoryName).Result.ToList();

            var repoInterface = repositoryInterfaces.FirstOrDefault(r => r.Identifier.Text != null
                                                                         && r.Identifier.Text.IndexOf("I", StringComparison.Ordinal) == 0
                                                                         && r.Identifier.Text.Remove(0, 1) == doClass.Identifier + _config.RepositorySuffix);
            if (repoInterface == null)
            {
                list.Add(CreateWithAnalysisError(className, _config.RepositorySuffix, "Class not implemented true named repository interface."));
                return list;
            }
            
            repositoryInfo.IsTenantRelated = !repositoryInterfaces.Any(i => i.Identifier.ToString().Contains("ITenantUnrelated"));
            
            // Repository interface info
            repositoryInfo.RepositoryInterfaceName = repoInterface.Identifier.Text;

            #endregion

            #region Repository model attributes

            #region DataAccess

            var dataAccess = (DataAccessAttribute)dataAccessAttr;

            var tableName = dataAccess.TableName ?? doClass.Identifier.ToString() + 's';
            repositoryInfo.TableName = tableName;

            // Get filters
            var filterKeys = (new[] {dataAccess.FilterKey1, dataAccess.FilterKey2, dataAccess.FilterKey3})
                .Where(fk => fk != null)
                .Select(fk =>
                {
                    // Filter data may be combined (ItemId, GroupId)
                    var filters = fk.Split(',').Select(f =>
                    {
                        var parameter = doClass.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(cp => (SyntaxAnalysisHelper.GetterInCodePropertyExist(cp) && SyntaxAnalysisHelper.SetterInCodePropertyExist(cp) && cp.Identifier.Text == f));
                        return new ParameterInfo(fk, parameter != null ? parameter.Type.ToFullString() : null);
                    }).ToList();
                    return new FilterInfo(string.Join("And", fk.Split(',')), filters);
                });

            // Common filter - isDeleted
            repositoryInfo.SpecialOptions = new FilterInfo("IsDeleted", new List<ParameterInfo> {new ParameterInfo("IsDeleted", typeof(bool).Name, Convert.ToString(dataAccess.IsDeleted).FirstSymbolToLower())});

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
                var cacheRepository = new CodeClassGeneratorCacheRepository()
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


            var properties = doClass.Members.OfType<PropertyDeclarationSyntax>().Where(cp => (SyntaxAnalysisHelper.GetterInCodePropertyExist(cp) && SyntaxAnalysisHelper.SetterInCodePropertyExist(cp))).ToList();

            // Add sql column name - skip members marked [DbIgnoreAttribute]
            var elements = properties
                .Where(p => !p.AttributeLists.Any() || !p.AttributeLists.First().Attributes.Any(a => a.Name.ToString().Contains(RepositoryDataModelHelper.DbIgnoreAttributeName)))
                .Select(p => p.Identifier.Text);
            repositoryInfo.Elements.AddRange(elements);


            // Primary keys info
            var primaryKeys = properties
                .Where(p => p.AttributeLists.Any() && p.AttributeLists.First().Attributes.Any(a => a.Name.ToString().Contains(RepositoryDataModelHelper.KeyAttributeName) && !a.Name.ToString().Contains(RepositoryDataModelHelper.VesionKeyAttributeName)))
                .Select(p => new ParameterInfo(p.Identifier.Text, p.Type.ToFullString()));
            repositoryInfo.PrimaryKeys.AddRange(primaryKeys);

            // Version key
            var versionKey = properties
                .Where(p => p.AttributeLists.Any() && p.AttributeLists.First().Attributes.Any(a => a.Name.ToString().Contains(RepositoryDataModelHelper.VesionKeyAttributeName)))
                .Select(p => p.Identifier.Text)
                .FirstOrDefault();
            repositoryInfo.VersionKey = versionKey;

            // Many to many
            var many2ManyInfos = properties
                .Where(p => p.AttributeLists.Any() && p.AttributeLists.First().Attributes.Any(a => a.Name.ToString().Contains(RepositoryDataModelHelper.DataMany2ManyAttributeName)))
                .SelectMany(p => SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(p))
                .Select(p => new Tuple<string,DataMany2ManyAttribute>(p.OwnerElementName, (DataMany2ManyAttribute)p))
                .Select(a => new Many2ManyInfo(a.Item1, a.Item2.ManyToManyEntytyType.Split('.').Last(), a.Item2.EntityType.Split('.').Last()));

            repositoryInfo.Many2ManyInfo.AddRange(many2ManyInfos);

            #endregion

            #region Custom repository

            // Check exist custom repository class
            var customRepository = SolutionSyntaxWalker.FindClassByName(_solution, _config.ProjectName, doClass.Identifier + _config.RepositorySuffix, _config.RepositoryTargetFolder).Result;
            var customRepoExist = customRepository != null;

            List<MethodDeclarationSyntax> customRepositoryMethods = null;

            // Get custom repository class info
            if (customRepoExist)
            {
                // Custom repository info
                customRepositoryMethods = SolutionSyntaxWalker.GetMethodsFromMembers(customRepository.Members.ToList());
                // Check constructor exist
                repositoryInfo.IsConstructorImplemented = SyntaxAnalysisHelper.ConstructorImplementationExist(customRepository);

                // Get repositories NameSpace
                var compilation = CSharpCompilation.Create("compilation").AddSyntaxTrees(customRepository.SyntaxTree);
                var model = compilation.GetSemanticModel(customRepository.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(customRepository);
                var fullName = symbol.ToString();
                repositoryInfo.RepositoryNamespace = fullName.Replace("." + customRepository.Identifier.Text, "");
            }

            #endregion

            #region Repository methods

            if (repositoryInfo.RepositoryNamespace == null)
            {
                repositoryInfo.RepositoryNamespace = _config.RepositoryMainPlace;
            }

            // Search method for implementation
            var repositoryInterfaceMethods = SolutionSyntaxWalker.GetMethodsFromMembers(repoInterface.Members.ToList());
            repositoryInfo.InterfaceMethodNames.AddRange(repositoryInterfaceMethods.Select(m => m.Identifier.Text));

            if(customRepositoryMethods!=null)
            {
                repositoryInfo.CustomRepositoryMethodNames.AddRange(customRepositoryMethods.Select(m => m.Identifier.Text));
            }

            #endregion

            // return one (general repository) or list of repository (versioned)
            return list;
        }

        #endregion


        #region Private

        private static IEnumerable<MethodImplementationInfo> GetUnimplementedMethods(RepositoryInfo info)
        {
            // Check implemented methods in custom repository
            var unimplemented = info.InterfaceMethodNames.Where(im => info.CustomRepositoryMethodNames.FirstOrDefault(cm => cm == im) == null);

            // Set methods which possible to generate

            // Add common methods
            var methods = new List<MethodImplementationInfo>
            {
                new MethodImplementationInfo() { Method = RepositoryMethod.GetAll, RequiresImplementation = false},
                new MethodImplementationInfo() { Method = RepositoryMethod.Insert, RequiresImplementation = false},
            };

            // Methods by keys from model (without methods from base model)
            foreach (var method in info.PossibleKeysForMethods)
            {
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.GetBy, Key = method.Key, Parameters = method.Parameters, RequiresImplementation = false });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.UpdateBy, Key = method.Key, RequiresImplementation = false });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.RemoveBy, Key = method.Key, Parameters = method.Parameters, RequiresImplementation = false });
            }

            // Set methods to implementation from possible list
            foreach (var um in unimplemented)
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

        #endregion
    }
}
