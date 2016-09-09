using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionedRepositoryGeneration.Generator.Analysis;
using VersionedRepositoryGeneration.Generator.Core;
using VersionedRepositoryGeneration.Generator.Heplers;
using VersionedRepositoryGeneration.Generator.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;
using SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace VersionedRepositoryGeneration.Generator.Services
{
    internal class RepositoryService
    {
        #region Private

        private readonly RepositoryProject _config;
        private readonly SolutionSyntaxWalker SolutionSyntaxWalker;

        #endregion

        #region Constructor

        public RepositoryService(RepositoryGeneratorWorkSpace workSpace, RepositoryProject config)
        {
            _config = config;

            var repositoryModelsProjects = _config.RepositoryClassProjects.Split(',').Select(p => p.Trim()).ToList();
            var repositoryInterfaceProjects = _config.RepositoryInterfacesProjectName.Split(',').Select(p => p.Trim()).ToList();

            var additionalProjectsForAnalysis = _config.AdditionalProjects.Split(',').Select(p => p.Trim()).ToList();
            additionalProjectsForAnalysis.Add(_config.TargetProjectName);

            SolutionSyntaxWalker = new SolutionSyntaxWalker(workSpace.Solution, repositoryModelsProjects, _config.RepositoryAttributeName, repositoryInterfaceProjects,_config.TargetProjectName, additionalProjectsForAnalysis);
        }

        #endregion

        #region Find versioned repositories

        public IEnumerable<ICodeClassGeneratorRepository> GetRepositories()
        {
            // Get all clases marked RepositoryAttribute
            var findClasses = SolutionSyntaxWalker.GetRepositoryClasses(RepositoryDataModelHelper.DataAccessAttributeName);

            var versionedRepositories = findClasses.SelectMany(c=> GetAllRepositories(c)).Where(c=> c!=null);

            var listOfSimilarClasses = versionedRepositories.Where(c => c.RepositoryAnalysisError == null).ToList();

            // Apply info from base clasess
            foreach (var r in listOfSimilarClasses)
            {
                var repositoryInfo = r.RepositoryInfo;
                
                // Find base class
                if (repositoryInfo.BaseClassName != null)
                {
                    var baseClass = SolutionSyntaxWalker.FindCustomRepositoryClassByName(repositoryInfo.BaseClassName);

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
            foreach (var info in resultRepositories.Where(r => !r.RepositoryName.Contains(CodeClassGeneratorVersionsRepository.RepositoryKind) && !r.RepositoryName.Contains(CodeClassGeneratorCacheRepository.RepositoryKind)).Select(r=>r.RepositoryInfo))
            {
                var methods = GetUnimplementedMethods(info.InterfaceMethodNames, info.CustomRepositoryMethodNames, info.PossibleKeysForMethods);
                info.MethodImplementationInfo.AddRange(methods);

                if (info.IsVersioning)
                {
                    var cacheRepoMethods = GetUnimplementedMethods(info.InterfaceMethodNames, info.CustomCacheRepositoryMethodNames, info.PossibleKeysForMethods);
                    info.CacheRepositoryMethodImplementationInfo.AddRange(cacheRepoMethods);
                }

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
                ClassFullName = SolutionSyntaxWalker.GetFullRepositoryModelName(doClass),
            };

            repositoryInfo.IsTenantRelated = !doClass.BaseTypeExist("ITenantUnrelated");

            // Base class name
            if (doClass.BaseList != null && doClass.BaseList.Types.Any())
            {
                var baseClass = doClass.BaseList.Types.FirstOrDefault(t => SolutionSyntaxWalker.IsBaseClass(t));

                if (baseClass!=null)
                {
                    repositoryInfo.BaseClassName = baseClass.ToString();
                }
            }

            #region Repository interface

            // Get implemented interfaces for custom repository (for example - IMenuItemRepository)

            var repositoryInterfaces = SolutionSyntaxWalker.GetInheritedInterfaces(repositoryInfo.GenericRepositoryInterfaceName).ToList();

            var repoInterface = repositoryInterfaces.FirstOrDefault(r => r.Identifier.Text != null
                                                                         && r.Identifier.Text.IndexOf("I", StringComparison.Ordinal) == 0
                                                                         && r.Identifier.Text.Remove(0, 1) == doClass.Identifier + _config.RepositorySuffix);
            if (repoInterface == null)
            {
                list.Add(CreateWithAnalysisError(className, _config.RepositorySuffix, "Class not implemented true named repository interface."));
                return list;
            }

            // Repository interface info
            repositoryInfo.RepositoryInterfaceName = repoInterface.Identifier.Text;

            #endregion

            #region Repository model attributes

            #region DataAccess

            var dataAccess = (DataAccessAttribute)dataAccessAttr;

            repositoryInfo.Identity = dataAccess.Identity;

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
                        var parameter = doClass.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(cp => cp.GetterExist() && cp.SetterExist() && cp.Identifier.Text == f);
                        var fullTypeName = SolutionSyntaxWalker.GetFullPropertyTypeName(parameter);
                        return new ParameterInfo(f, parameter != null ? fullTypeName : null);
                    }).ToList();
                    return new FilterInfo(string.Join("And", fk.Split(',')), filters, FilterType.FilterKey);
                });

            repositoryInfo.FilterInfos.AddRange(filterKeys);

            // Common filter - isDeleted
            repositoryInfo.SpecialOptions = new FilterInfo("IsDeleted", new List<ParameterInfo> { new ParameterInfo("IsDeleted", "bool", Convert.ToString(dataAccess.IsDeleted).FirstSymbolToLower()) }, FilterType.FilterKey);

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


            var properties = doClass.Members.OfType<PropertyDeclarationSyntax>().Where(cp => cp.GetterExist() && cp.SetterExist()).ToList();

            // Add sql column name - skip members marked [DbIgnoreAttribute]
            var elements = properties
                .Where(p => !p.AttributeExist(RepositoryDataModelHelper.DbIgnoreAttributeName))
                .Select(p => p.Identifier.Text);
            repositoryInfo.Elements.AddRange(elements);

            // Primary keys info
            var primaryKeys = properties
                .Where(p => p.AttributeExist(RepositoryDataModelHelper.KeyAttributeName))
                .Select(p =>
                {
                    var fullTypeName = SolutionSyntaxWalker.GetFullPropertyTypeName(p);
                    return new ParameterInfo(p.Identifier.Text, fullTypeName);
                });

            repositoryInfo.PrimaryKeys.AddRange(primaryKeys);

            // Version key
            var versionKey = properties
                .Where(p => p.AttributeExist(RepositoryDataModelHelper.VesionKeyAttributeName))
                .Select(p => p.Identifier.Text)
                .FirstOrDefault();
            repositoryInfo.VersionKey = versionKey;

            // Many to many
            var many2ManyInfos = properties
                .Where(p => p.AttributeExist(RepositoryDataModelHelper.DataMany2ManyAttributeName))
                .SelectMany(p => SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(p))
                .Select(p => new Tuple<string,DataMany2ManyAttribute>(p.OwnerElementName, (DataMany2ManyAttribute)p))
                .Select(a => new Many2ManyInfo(a.Item1, a.Item2.ManyToManyEntytyType.Split('.').Last(), a.Item2.EntityType.Split('.').Last()));

            repositoryInfo.Many2ManyInfo.AddRange(many2ManyInfos);

            #endregion

            #region Custom repository

            // Check exist custom repository class
            var customRepository = SolutionSyntaxWalker.FindCustomRepositoryClassByName(doClass.Identifier + _config.RepositorySuffix).FirstOrDefault();
            var customRepoExist = customRepository != null;

            List<MethodDeclarationSyntax> customRepositoryMethods = null;

            // Get custom repository class info
            if (customRepoExist)
            {
                // Custom repository info
                customRepositoryMethods = customRepository.Members.OfType<MethodDeclarationSyntax>().ToList();
                // Check constructor exist
                repositoryInfo.IsConstructorImplemented = customRepository.ConstructorImplementationExist();

                repositoryInfo.RepositoryNamespace = SolutionSyntaxWalker.GetCustomRepositoryNamespace(customRepository);
            }

            var customCacheRepository = SolutionSyntaxWalker.FindCustomRepositoryClassByName(doClass.Identifier + "Cache" + _config.RepositorySuffix).FirstOrDefault();
            var customCacheRepositoryExist = customCacheRepository != null;

            List<MethodDeclarationSyntax> customCacheRepositoryMethods = null;
            // Get custom cache repository class info
            if (customCacheRepositoryExist)
            {
                // Custom repository info
                customCacheRepositoryMethods = customCacheRepository.Members.OfType<MethodDeclarationSyntax>().ToList();
                // Check constructor exist
                repositoryInfo.IsCacheRepositoryConstructorImplemented = customCacheRepository.ConstructorImplementationExist();
            }

            #endregion

            #region Repository methods

            if (repositoryInfo.RepositoryNamespace == null)
            {
                repositoryInfo.RepositoryNamespace = _config.TargetProjectName;
            }

            // Search method for implementation
            var repositoryInterfaceMethods = repoInterface.Members.OfType<MethodDeclarationSyntax>().ToList();
            repositoryInfo.InterfaceMethodNames.AddRange(repositoryInterfaceMethods.Select(m => m.Identifier.Text));

            if(customRepositoryMethods!=null)
            {
                repositoryInfo.CustomRepositoryMethodNames.AddRange(customRepositoryMethods.Select(m => m.Identifier.Text));
            }
            if (customCacheRepositoryMethods != null)
            {
                repositoryInfo.CustomCacheRepositoryMethodNames.AddRange(customCacheRepositoryMethods.Select(m => m.Identifier.Text));
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
            var unimplemented = info.InterfaceMethodNames.Where(im => info.CustomRepositoryMethodNames.FirstOrDefault(cm => cm == im || cm + "Async" == im) == null);

            // Set methods which possible to generate

            // Add common methods
            var methods = new List<MethodImplementationInfo>
            {
                new MethodImplementationInfo() { Method = RepositoryMethod.GetAll},
                new MethodImplementationInfo() { Method = RepositoryMethod.Insert}
            };

            // Methods by keys from model (without methods from base model)
            foreach (var filterInfo in info.PossibleKeysForMethods)
            {
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.GetBy, FilterInfo = filterInfo});
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.UpdateBy, FilterInfo = filterInfo});
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.RemoveBy, FilterInfo = filterInfo});
            }

            // Set methods to implementation from possible list
            foreach (var um in unimplemented)
            {
                // Seach in possible list
                var m = methods.FirstOrDefault(methodInfo => NameIsTrue(methodInfo, um));
                if (m == null) continue;
                // Set to implementation
                m.RequiresImplementation = true;
            }

            return methods;
        }

        private static IEnumerable<MethodImplementationInfo> GetUnimplementedMethods(List<string> interfaceMethodNames, List<string>  customRepositoryMethodNames, List<FilterInfo> possibleKeysForMethods)
        {
            // Check implemented methods in custom repository
            var unimplemented = interfaceMethodNames.Where(im => customRepositoryMethodNames.FirstOrDefault(cm => cm == im || cm + "Async" == im) == null);

            // Set methods which possible to generate

            // Add common methods
            var methods = new List<MethodImplementationInfo>
            {
                new MethodImplementationInfo() { Method = RepositoryMethod.GetAll},
                new MethodImplementationInfo() { Method = RepositoryMethod.Insert}
            };

            // Methods by keys from model (without methods from base model)
            foreach (var filterInfo in possibleKeysForMethods)
            {
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.GetBy, FilterInfo = filterInfo });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.UpdateBy, FilterInfo = filterInfo });
                methods.Add(new MethodImplementationInfo() { Method = RepositoryMethod.RemoveBy, FilterInfo = filterInfo });
            }

            // Set methods to implementation from possible list
            foreach (var um in unimplemented)
            {
                // Seach in possible list
                var m = methods.FirstOrDefault(methodInfo => NameIsTrue(methodInfo, um));
                if (m == null) continue;
                // Set to implementation
                m.RequiresImplementation = true;
            }

            return methods;
        }

        private static bool NameIsTrue(MethodImplementationInfo methodInfo, string name)
        {
            return methodInfo.Method.GetName() + (methodInfo.FilterInfo != null ? methodInfo.FilterInfo.Key ?? "" : "") == name || methodInfo.Method.GetName() + (methodInfo.FilterInfo != null ? methodInfo.FilterInfo.Key ?? "" : "") + "Async" == name;
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
