using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.RepositoriesGeneration.Analysis;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Core;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;
using MethodInfo = WCFGenerator.RepositoriesGeneration.Infrastructure.MethodInfo;
using ParameterInfo = WCFGenerator.RepositoriesGeneration.Infrastructure.ParameterInfo;

namespace WCFGenerator.RepositoriesGeneration.Services
{
    internal class RepositoryService
    {
        #region Private

        private readonly RepositoryProject _config;
        private readonly SolutionSyntaxWalker _solutionSyntaxWalker;

        #endregion

        #region Constructor

        public RepositoryService(GeneratorWorkspace workSpace, RepositoryProject config)
        {
            _config = config;

            var repositoryModelsProjects = _config.RepositoryClassProjects.Split(',').Select(p => p.Trim()).ToList();
            var repositoryInterfaceProjects = _config.RepositoryInterfacesProjectName.Split(',').Select(p => p.Trim()).ToList();

            var additionalProjectsForAnalysis = _config.AdditionalProjects.Split(',').Select(p => p.Trim()).ToList();
            additionalProjectsForAnalysis.Add(_config.TargetProjectName);

            _solutionSyntaxWalker = new SolutionSyntaxWalker(workSpace.Solution, repositoryModelsProjects, _config.RepositoryAttributeName, repositoryInterfaceProjects, _config.TargetProjectName, additionalProjectsForAnalysis);
        }

        #endregion

        #region Find versioned repositories

        public IEnumerable<ICodeClassGeneratorRepository> GetRepositories(RepositoryProject config)
        {
            // Get all clases marked RepositoryAttribute
            var findClasses = _solutionSyntaxWalker.GetRepositoryClasses();
            // Get all candidates for generate repository
            var candidatesOfRepositories = findClasses.Select(c => GetRepository(c)).Where(c => c != null).ToList();
            // Skip with error
            var correctlyRepositories = candidatesOfRepositories.Where(c => c.RepositoryInfo.CanBeExtendedAnalysis).ToList();
            
            var dataAccessServiceNamespace = _solutionSyntaxWalker.GetFullTypeName("IDataAccessService");
            var dataControllerServiceNamespace = _solutionSyntaxWalker.GetFullTypeName("IDataAccessController");
            var dateTimeServiceNamespace = _solutionSyntaxWalker.GetFullTypeName("IDateTimeService");
            var repositorybase = string.IsNullOrEmpty(config.RepositoryBase) ? "RepositoryBase" : config.RepositoryBase;

            // Apply info from base clasess
            foreach (var r in correctlyRepositories)
            {
                var repositoryInfo = r.RepositoryInfo;

                // Find base class
                if (repositoryInfo.BaseClassName != null)
                {
                    var baseSimilarClass = correctlyRepositories.FirstOrDefault(x => x.RepositoryInfo.ClassName == repositoryInfo.BaseClassName);

                    if (baseSimilarClass != null)
                    {
                        baseSimilarClass.RepositoryInfo.IsJoned = true;
                        repositoryInfo.JoinRepositoryInfo = baseSimilarClass.RepositoryInfo;
                    }
                }

                repositoryInfo.DataAccessServiceTypeName = dataAccessServiceNamespace;
                repositoryInfo.DataAccessControllerTypeName = dataControllerServiceNamespace;
                repositoryInfo.DateTimeServiceTypeName = dateTimeServiceNamespace;
                repositoryInfo.RepositoryBaseTypeName = repositorybase;
            }

            // Skip not generated
            var canBeGeneratedRepositories = correctlyRepositories.Where(r => r.RepositoryInfo.CanBeGenerated).ToList();

            // Fill method implementation info for main repositories
            foreach (var info in canBeGeneratedRepositories.Select(r => r.RepositoryInfo))
            {
                var methods = GetUnimplementedMethods(info.InterfaceMethods, info.CustomRepositoryMethodNames, info.PossibleKeysForMethods, info.ClassFullName);
                info.MethodImplementationInfo.AddRange(methods);

                if (info.IsVersioning)
                {
                    var cacheRepoMethods = GetUnimplementedMethods(info.InterfaceMethods, info.CustomCacheRepositoryMethodNames, info.PossibleKeysForMethods, info.ClassFullName);
                    info.CacheRepositoryMethodImplementationInfo.AddRange(cacheRepoMethods);
                }
            }

            // Fill "many-to-many" info
            var many2ManyInfos = canBeGeneratedRepositories.SelectMany(repository => repository.RepositoryInfo.Many2ManyInfo);

            foreach (var many2Many in many2ManyInfos)
            {
                // get namespaces for generate reference in repository
                many2Many.RepositoryNamespaces = canBeGeneratedRepositories
                    .Where(r => (r.RepositoryName.StartsWith(many2Many.EntityType) || r.RepositoryName.StartsWith(many2Many.ManyToManyEntytyType)) && r.RepositoryName.EndsWith(r.RepositoryInfo.RepositorySuffix) && r.RepositoryInfo.RepositoryNamespace != null)
                    .Select(r => r.RepositoryInfo.RepositoryNamespace).Distinct().ToList();

                // get repository info by  EntityType from [many2manyAttribute]
                many2Many.EntityRepositoryInfo = canBeGeneratedRepositories
                    .Where(r => r.RepositoryName == many2Many.EntityType.Split('.').Last() + r.RepositoryInfo.RepositorySuffix)
                    .Select(r => r.RepositoryInfo)
                    .FirstOrDefault();

                // get repository info by ManyToManyEntytyType from [many2manyAttribute]
                var manyToManyRepositoryInfo = canBeGeneratedRepositories
                    .Where(r => r.RepositoryName == many2Many.ManyToManyEntytyType.Split('.').Last() + r.RepositoryInfo.RepositorySuffix)
                    .Select(r => r.RepositoryInfo)
                    .FirstOrDefault();
                if (manyToManyRepositoryInfo != null)
                {
                    manyToManyRepositoryInfo.IsManyToMany = true;
                }
                many2Many.ManyToManyRepositoryInfo = manyToManyRepositoryInfo;
            }

            // Add versioned repository
            var versioned = canBeGeneratedRepositories.Where(r => r.RepositoryInfo.IsVersioning).SelectMany(r =>
            {
                #region Search required namespace

                var requiredNamespacesForService = new List<string>();

                // add namespace of repositories which added from relation many to many 
                var manyToMany = r.RepositoryInfo.Many2ManyInfo.SelectMany(i => i.RepositoryNamespaces).ToList();
                requiredNamespacesForService.AddRange(manyToMany);
                r.RepositoryInfo.RequiredNamespaces.Add(RepositoryType.VersionService, requiredNamespacesForService.Distinct().ToList());

                #endregion

                var list = new List<RepositoryCodeGeneratorAbstract>
                {
                    // version repository
                    new VersionsRepositoryCodeGenerator {RepositoryInfo = r.RepositoryInfo},
                    // cache repository
                    new CacheRepositoryCodeGenerator {RepositoryInfo = r.RepositoryInfo}
                };
                // Skip service-repository for many2many model
                if(!r.RepositoryInfo.IsManyToMany)
                {
                    list.Add(new CodeClassGeneratorVersionedRepositoryService {RepositoryInfo = r.RepositoryInfo});
                }

                return list;
            });

            canBeGeneratedRepositories = canBeGeneratedRepositories.Where(r => !r.RepositoryInfo.IsVersioning).Concat(versioned).ToList();

            // Skiped with error
            var repositoriesWithErrors = candidatesOfRepositories.Where(c => !c.RepositoryInfo.CanBeExtendedAnalysis).ToList();

            return canBeGeneratedRepositories.Concat(repositoriesWithErrors).ToList();
        }

        #endregion

        #region Analysis methods

        /// <summary>
        ///     Get general info about candidate for repository
        /// </summary>
        /// <param name="doClass">Class syntax</param>
        private RepositoryCodeGeneratorAbstract GetRepository(ClassDeclarationSyntax doClass)
        {
            var className = doClass.Identifier.Text;

            // Get DataAccess attribute from repository model
            var dataAccessAttr = SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(doClass).FirstOrDefault(x => x.Name.ToString().Contains(_config.RepositoryAttributeName));

            // common repository info
            var repositoryInfo = new RepositoryInfo
            {
                RepositorySuffix = _config.RepositorySuffix,
                ClassName = className,
                ClassFullName = _solutionSyntaxWalker.GetFullRepositoryModelName(doClass),
                IsTenantRelated = !doClass.BaseTypeExist("ITenantUnrelated"),
                IsStoreDependent = doClass.BaseTypeExist("IStoreRelated") && doClass.Members.OfType<PropertyDeclarationSyntax>().Any(x => x.Identifier.Text == "StoreId")
            };


            // Base class name
            if (doClass.BaseList != null && doClass.BaseList.Types.Any())
            {
                var baseClass = doClass.BaseList.Types.FirstOrDefault(t => _solutionSyntaxWalker.IsBaseClass(t));

                if (baseClass != null)
                {
                    repositoryInfo.BaseClassName = baseClass.ToString();
                }
            }

            #region Repository model attributes

            #region DataAccess

            var dataAccess = (DataAccessAttribute)dataAccessAttr;

            var tableName = dataAccess.TableName ?? doClass.Identifier.ToString() + 's';
            repositoryInfo.TableName = tableName;

            // Get filters
            var filterKeys = new[] {dataAccess.FilterKey1, dataAccess.FilterKey2, dataAccess.FilterKey3}
                .Where(fk => fk != null)
                .Select(fk =>
                {
                    // Filter data may be combined (ItemId, GroupId)
                    var filters = fk.Split(',').Select(f =>
                    {
                        var name = f.Trim();
                        var parameter = doClass.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(cp => cp.GetterExist() && cp.SetterExist() && cp.Identifier.Text == name);
                        if(parameter == null)
                        {
                            return null;
                        }
                        var fullTypeName = _solutionSyntaxWalker.GetFullPropertyTypeName(parameter);
                        var needGeneratePeriod = fullTypeName == "System.DateTime" || fullTypeName == "System.DateTimeOffset";
                        return new ParameterInfo(name, parameter != null ? fullTypeName : null, needGeneratePeriod);
                    }).ToList();
                    return new FilterInfo(string.Join("And", fk.Split(',').Select(f => f.Trim())), filters, FilterType.FilterKey);
                });

            repositoryInfo.FilterInfos.AddRange(filterKeys);

            // Common filter - isDeleted, modified
            if(dataAccess.IsDeleted.HasValue)
            {
                repositoryInfo.SpecialOptionsIsDeleted = new FilterInfo("IsDeleted",
                    new List<ParameterInfo>
                    {
                        new ParameterInfo("IsDeleted", "bool", false, Convert.ToString(dataAccess.IsDeleted).FirstSymbolToLower())
                    }, FilterType.FilterKey);
            }
            repositoryInfo.SpecialOptionsModified = new FilterInfo("Modified", new List<ParameterInfo>
            {
                new ParameterInfo("Modified", "DateTimeOffset", false)
            }, FilterType.FilterKey);
            
            repositoryInfo.VersionTableName = dataAccess.TableVersion;
            var isVersioning = dataAccess.TableVersion != null;
            repositoryInfo.IsVersioning = isVersioning;
            repositoryInfo.Identity = dataAccess.Identity;

            #endregion

            #endregion

            #region Property attributes

            var properties = doClass.Members.OfType<PropertyDeclarationSyntax>().Where(cp => cp.GetterExist() && cp.SetterExist()).ToList();

            // Add sql column name - skip members marked [DbIgnoreAttribute]
            var elements = properties
                .Select(p =>
                {
                    var typeName = p.Type.ToString();
                    
                    return new PropertyInfo(
                        name: p.Identifier.Text,
                        isParameter: typeName == "string",
                        isNullable: typeName.EndsWith("?"),
                        isBool: typeName.Contains("bool"),
                        isEnum: _solutionSyntaxWalker.PropertyIsEnum(p),
                        cultureDependent: typeName.Contains("decimal") || typeName.Contains("double") || typeName.Contains("float") || typeName.Contains("DateTime"),
                        ignoreOptions: GetIgnoreOptions(doClass, p));
                })
                .Where(info => !info.IgnoreAlways);

            repositoryInfo.Elements.AddRange(elements);

            // Primary keys info
            var primaryKeys = properties
                .Where(p => p.AttributeExist(RepositoryDataModelHelper.KeyAttributeName))
                .Select(p =>
                {
                    var fullTypeName = _solutionSyntaxWalker.GetFullPropertyTypeName(p);
                    return new ParameterInfo(p.Identifier.Text, fullTypeName, false);
                });

            repositoryInfo.PrimaryKeys.AddRange(primaryKeys);

            // Version key
            var versionKey = properties.FirstOrDefault(p => p.AttributeExist(RepositoryDataModelHelper.VesionKeyAttributeName));

            if (versionKey != null)
            {
                var keyName = versionKey.Identifier.Text;
                repositoryInfo.VersionKeyName = keyName;
                var fullTypeName = _solutionSyntaxWalker.GetFullPropertyTypeName(versionKey);

                // add filter parameter
                repositoryInfo.VersionKey = new ParameterInfo(keyName, fullTypeName, false);
            }

            // Many to many
            var many2ManyInfos = properties
                .Where(p => p.AttributeExist(RepositoryDataModelHelper.DataMany2ManyAttributeName))
                .SelectMany(p => SyntaxAnalysisHelper.GetAttributesAndPropepertiesCollection(p))
                .Where(p => p.Name == RepositoryDataModelHelper.DataMany2ManyAttributeName)
                .Select(p => new Tuple<string, DataMany2ManyAttribute>(p.OwnerElementName, (DataMany2ManyAttribute)p))
                .Select(a =>
                new Many2ManyInfo(a.Item1, a.Item2.ManyToManyEntytyType, a.Item2.EntityType));

            repositoryInfo.Many2ManyInfo.AddRange(many2ManyInfos);

            #endregion

            #region Custom repository

            // Check exist custom repository class
            var customRepository = _solutionSyntaxWalker.FindCustomRepositoryClassByName(doClass.Identifier + _config.RepositorySuffix).FirstOrDefault();
            var customRepoExist = customRepository != null;

            List<MethodInfo> customRepositoryMethods = null;

            // Get custom repository class info
            if (customRepoExist)
            {
                // Custom repository info
                customRepositoryMethods = customRepository.Members.OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo() { Name = m.Identifier.Text, ReturnType = m.ReturnType.ToString() })
                    .ToList();
                // Check constructor exist
                repositoryInfo.IsConstructorImplemented = customRepository.ConstructorImplementationExist();

                repositoryInfo.RepositoryNamespace = _solutionSyntaxWalker.GetCustomRepositoryNamespace(customRepository);
            }

            var customCacheRepository = _solutionSyntaxWalker.FindCustomRepositoryClassByName(doClass.Identifier + "Cache" + _config.RepositorySuffix).FirstOrDefault();
            var customCacheRepositoryExist = customCacheRepository != null;

            List<MethodInfo> customCacheRepositoryMethods = null;
            // Get custom cache repository class info
            if (customCacheRepositoryExist)
            {
                // Custom repository info
                customCacheRepositoryMethods = customCacheRepository.Members.OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo() { Name = m.Identifier.Text, ReturnType = m.ReturnType.ToString() })
                    .ToList();

                // Check constructor exist
                repositoryInfo.IsCacheRepositoryConstructorImplemented = customCacheRepository.ConstructorImplementationExist();

                if (repositoryInfo.RepositoryNamespace == null)
                {
                    repositoryInfo.RepositoryNamespace = _solutionSyntaxWalker.GetCustomRepositoryNamespace(customCacheRepository);
                }
            }

            #endregion

            #region Repository interface

            // Get implemented interfaces for custom repository (for example - IMenuItemRepository : IRepository<MenuItem>)
            var repoInterface = _solutionSyntaxWalker.GetInheritedInterface(repositoryInfo.GenericRepositoryInterfaceName);

            if (repoInterface == null)
            {
                repositoryInfo.RepositoryAnalysisError.Add(new AnalysisError(RepositoryAnalysisError.InterfaceNotFound, string.Format("Repository interface {0} not found.", repositoryInfo.GenericRepositoryInterfaceName)));
            }
            else
            {
                // Repository interface info
                repositoryInfo.RepositoryInterfaceName = _solutionSyntaxWalker.GetTypeNamespace(repoInterface) + "." + repoInterface.Identifier.Text;

                #region Repository methods

                // Search method for implementation
                var repositoryInterfaceMethods = repoInterface.Members.OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo()
                    {
                        Name = m.Identifier.Text,
                        ReturnType = m.ReturnType.ToString(),
                        Parameters = m.ParameterList.Parameters.Select(p => new ParameterInfo(p.Identifier.ToString(), p.Type.ToString(), false)).ToList()
                    })
                    .ToList();

                repositoryInfo.InterfaceMethods.AddRange(repositoryInterfaceMethods);

                if (customRepositoryMethods != null)
                {
                    repositoryInfo.CustomRepositoryMethodNames.AddRange(customRepositoryMethods);
                }
                if (customCacheRepositoryMethods != null)
                {
                    repositoryInfo.CustomCacheRepositoryMethodNames.AddRange(customCacheRepositoryMethods);
                }

                #endregion
            }

            #endregion

            #region Search required namespace

            if (repositoryInfo.RepositoryNamespace == null)
            {
                repositoryInfo.RepositoryNamespace = _config.DefaultNamespace;
            }

            var requiredNamespaces = new List<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading.Tasks",
                "System.Globalization"
            };

            repositoryInfo.RequiredNamespaces.Add(RepositoryType.General, requiredNamespaces);

            if(repoInterface != null)
            {
                var repositoryInterfaceNamespace = _solutionSyntaxWalker.GetTypeNamespace(repoInterface);
                repositoryInfo.RequiredNamespaces.Add(RepositoryType.Cache, new List<string> { repositoryInterfaceNamespace });
            }

            repositoryInfo.DatabaseType = (DatabaseType)_config.DatabaseType;

            #endregion

            var repositoryAndDo = new RepositoryCodeGenerator
            {
                RepositoryInfo = repositoryInfo
            };
            
            // return general repository
            return repositoryAndDo;
        }

        #endregion

        #region Private

        private static IEnumerable<MethodImplementationInfo> GetUnimplementedMethods(List<MethodInfo> interfaceMethodNames, List<MethodInfo> customRepositoryMethodNames, List<FilterInfo> possibleKeysForMethods, string fullRepositoryModelName)
        {
            // Check implemented methods in custom repository
            var unimplemented = interfaceMethodNames
                .Where(im => customRepositoryMethodNames.FirstOrDefault(cm => cm.Name == im.Name) == null)
                .GroupBy(p=>p.Name)
                .Select(gr => gr.Count() == 1 ? gr.Single() : gr.FirstOrDefault(p => p.Parameters.First().TypeName != fullRepositoryModelName.Split('.').Last()))
                .ToList();

            // Set methods which possible to generate

            // Add common methods
            var methods = new List<MethodImplementationInfo>
            {
                new MethodImplementationInfo { Method = RepositoryMethod.GetAll },
                new MethodImplementationInfo { Method = RepositoryMethod.Insert },
                new MethodImplementationInfo { Method = RepositoryMethod.InsertOrUpdate},
                new MethodImplementationInfo { Method = RepositoryMethod.InsertMany },
            };
            try
            {

                // Methods by keys from model (without methods from base model)
                var keyBasedMethods = possibleKeysForMethods
                    .SelectMany(filterInfo => new List<MethodImplementationInfo>
                    {
                        new MethodImplementationInfo
                        {
                            Method = RepositoryMethod.GetBy,
                            ReturnType = "IEnumerable<" + fullRepositoryModelName + ">",
                            FilterInfo = filterInfo,
                            Parameters = interfaceMethodNames.FirstOrDefault(p => p.Name == ("GetBy" + filterInfo.Key) || p.Name == ("GetBy" + filterInfo.Key + "Async"))?.Parameters ?? filterInfo.Parameters
                        },
                        new MethodImplementationInfo {Method = RepositoryMethod.UpdateBy, FilterInfo = filterInfo},
                        new MethodImplementationInfo
                        {
                            Method = RepositoryMethod.RemoveBy,
                            FilterInfo = filterInfo,
                            Parameters = interfaceMethodNames.FirstOrDefault(p => p.Name == ("GetBy" + filterInfo.Key) || p.Name == ("GetBy" + filterInfo.Key + "Async"))?.Parameters ?? filterInfo.Parameters
                        }
                    });

                methods.AddRange(keyBasedMethods);
            }
            catch
            {
                Console.WriteLine("GetUnimplementedMethods error for: " + fullRepositoryModelName);
                throw;
            }
            // Set methods to implementation from possible list
            foreach (var um in unimplemented)
            {
                // Seach in possible list
                var mm = methods.Where(methodInfo => NameIsTrue(methodInfo, um.Name));
                if (!mm.Any()) continue;
                // Set to implementation
                foreach (var m in mm)
                {
                    m.RequiresImplementation = true;
                    m.Name = um.Name;
                    m.ReturnType = um.ReturnType;
                    m.Parameters = um.Parameters;
                }
            }

            return methods;
        }

        private static bool NameIsTrue(MethodImplementationInfo methodInfo, string name)
        {
            return methodInfo.Method.GetName() + (methodInfo.FilterInfo != null ? methodInfo.FilterInfo.Key ?? "" : "") == name || methodInfo.Method.GetName() + (methodInfo.FilterInfo != null ? methodInfo.FilterInfo.Key ?? "" : "") + "Async" == name;
        }

        private PropertyIgnoreOptions GetIgnoreOptions(ClassDeclarationSyntax doClass, PropertyDeclarationSyntax property)
        {
            if (!property.AttributeExist(RepositoryDataModelHelper.DbIgnoreAttributeName))
            {
                return PropertyIgnoreOptions.None;
            }

            var propertyName = property.Identifier.Text;

            var value = _solutionSyntaxWalker.GetAttributeArgumentValue(doClass, 
                propertyName,
                RepositoryDataModelHelper.DbIgnoreAttributeName,
                "IgnoreOnUpdate");

            if (value == null || !bool.TryParse(value, out bool ignoreOnUpdate))
            {
                return PropertyIgnoreOptions.Always;
            }

            return ignoreOnUpdate
                ? PropertyIgnoreOptions.OnUpdate
                : PropertyIgnoreOptions.Always;
        }

        #endregion
    }
}