using System;
using System.Collections.Generic;
using System.Linq;
using WCFGenerator.RepositoriesGeneration.Core.SQL;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class RepositoryInfo
    {
        #region Fields

        private string _versionKeyName;
        private string _primaryKeyName;
        private List<ParameterInfo> _primaryKeys;
        private ParameterInfo _versionKey;
        private const string _syncStateFiledName = "SyncState";

        #endregion

        #region Constructor

        public RepositoryInfo()
        {
            FilterInfos = new List<FilterInfo>();
            PrimaryKeys = new List<ParameterInfo>();
            Elements = new List<PropertyInfo>();
            Many2ManyInfo = new List<Many2ManyInfo>();
            InterfaceMethods = new List<MethodInfo>();
            CustomRepositoryMethodNames = new List<MethodInfo>();
            CustomCacheRepositoryMethodNames = new List<MethodInfo>();
            MethodImplementationInfo = new List<MethodImplementationInfo>();
            CacheRepositoryMethodImplementationInfo = new List<MethodImplementationInfo>();
            RequiredNamespaces = new Dictionary<RepositoryType, List<string>>();
            RepositoryAnalysisError = new List<AnalysisError>();
        }

        #endregion

        #region Repository model class info

        /// <summary>
        ///     Information about inherited model repository (join repository)
        /// </summary>
        public RepositoryInfo JoinRepositoryInfo { get; set; }

        /// <summary>
        ///     Suffix of repository name
        /// </summary>
        public string RepositorySuffix { get; set; }

        /// <summary>
        ///     Namespace of repository class
        /// </summary>
        public string RepositoryNamespace { get; set; }

        /// <summary>
        ///     Repository model type
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        ///     Name of base class (it joined to this)
        /// </summary>
        public string BaseClassName { get; set; }

        /// <summary>
        ///     Full type of repository model
        /// </summary>
        public string ClassFullName { get; set; }

        /// <summary>
        ///     Name of database table for repository model
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     Name of database table for version repository model
        /// </summary>
        public string VersionTableName { get; set; }

        /// <summary>
        ///     Property names - table columns
        /// </summary>
        public List<PropertyInfo> Elements { set; get; }

        /// <summary>
        ///    Dependent properties names - hidden table columns
        /// </summary>
        public List<PropertyInfo> HiddenElements
        {
            get
            {
                return IsTenantRelated
                    ? new List<PropertyInfo>() {new PropertyInfo("TenantId")}
                    : new List<PropertyInfo>();
            }
        }

        /// <summary>
        ///     Standart name of repository class
        /// </summary>
        public string RepositoryName
        {
            get { return ClassName + RepositorySuffix; }
        }

        /// <summary>
        ///     Returns the name of generic repository interface
        /// </summary>
        public string GenericRepositoryInterfaceName { get { return string.Format("I{0}<{1}>", RepositorySuffix, ClassName); } }

        /// <summary>
        ///     Name of repository interface
        /// </summary>
        public string RepositoryInterfaceName { get; set; }

        /// <summary>
        ///     Required namespaces
        /// </summary>
        public Dictionary<RepositoryType, List<string>> RequiredNamespaces { get; set; }

        /// <summary>
        ///     Primary Keys
        /// </summary>
        public List<ParameterInfo> PrimaryKeys
        {
            get
            {
                if (_primaryKeys == null)
                {
                    _primaryKeys = new List<ParameterInfo>();
                }
                if(_primaryKeys.Count == 0 && JoinRepositoryInfo != null && JoinRepositoryInfo.PrimaryKeys.Count !=0)
                {
                    _primaryKeys.AddRange(JoinRepositoryInfo.PrimaryKeys);
                }
                return _primaryKeys;
            }
            set { _primaryKeys = value; }
        }

        /// <summary>
        ///     Returns the name of the combined primary key 
        /// </summary>
        public string PrimaryKeyName
        {
            get
            {
                if (string.IsNullOrEmpty(_primaryKeyName))
                {
                    if(PrimaryKeys!=null && PrimaryKeys.Count != 0)
                    {
                        _primaryKeyName = string.Join("And", PrimaryKeys.Select(info => info.Name));
                    }
                    else if(JoinRepositoryInfo!=null)
                    {
                        _primaryKeyName = JoinRepositoryInfo.PrimaryKeyName;
                    }
                }
                return _primaryKeyName;
            }
        }

        /// <summary>
        ///     Return true if Identity of PK true
        /// </summary>
        public bool Identity { get; set; }

        /// <summary>
        ///     Filters
        /// </summary>
        public List<FilterInfo> FilterInfos { get; set; }

        /// <summary>
        ///     Special filter options (it is "isDeleted")
        /// </summary>
        public FilterInfo SpecialOptionsIsDeleted { get; set; }

        /// <summary>
        ///     Special filter options (by "Modified")
        /// </summary>
        public FilterInfo SpecialOptionsModified { get; set; }

        /// <summary>
        ///     Name of member of repository model which marked as version key (for versioned repository)
        /// </summary>
        public string VersionKeyName
        {
            get
            {
                return _versionKeyName ?? (JoinRepositoryInfo != null ? JoinRepositoryInfo.VersionKeyName : null);
            }
            set { _versionKeyName = value; }
        }

        ///// <summary>
        /////     Version key
        ///// </summary>
        public ParameterInfo VersionKey
        {
            get
            {
                return _versionKey ?? (JoinRepositoryInfo != null ? JoinRepositoryInfo.VersionKey : null);
            }
            set { _versionKey = value; }
        }

        /// <summary>
        ///     Return true if repository supported versioning
        /// </summary>
        public bool IsVersioning { get; set; }

        /// <summary>
        ///     Repository joined to another repository
        /// </summary>
        public bool IsJoned { get; set; }

        /// <summary>
        ///    Repository can be generated
        /// </summary>
        public bool CanBeGenerated
        {
            get
            {
                if (!IsJoned && !RepositoryAnalysisError.Any())
                    return true;
                if (IsJoned && !RepositoryInterfaceNotFound)
                    return true;
                return false;
            }
        }

        /// <summary>
        ///    Repository can be extended analysis
        /// </summary>
        public bool CanBeExtendedAnalysis
        {
            get
            {
                return RepositoryAnalysisError.Count == 1 && RepositoryInterfaceNotFound || !RepositoryAnalysisError.Any();
            }
        }

        /// <summary>
        ///     Repository joined to another repository as many-to-many and can not generate
        /// </summary>
        public bool IsManyToMany { get; set; }

        /// <summary>
        ///     Repository is tenant related
        /// </summary>
        public bool IsTenantRelated { get; set; }

        /// <summary>
        /// Repository is store dependent
        /// </summary>
        public bool IsStoreDependent { get; set; }

        /// <summary>
        ///     Return true if constructor implemented in custom repository 
        /// </summary>
        public bool IsConstructorImplemented { get; set; }

        /// <summary>
        ///     Return true if constructor implemented in custom cache repository (for versioned repository)
        /// </summary>
        public bool IsCacheRepositoryConstructorImplemented { get; set; }

        public bool HasSyncState { get; set; }


        /// <summary>
        ///     Return true if need create argument IsDeleted in "Get" methods
        /// </summary>
        public bool IsDeletedExist
        {
            get
            {
               return SpecialOptionsIsDeleted != null;
            }
        }

        /// <summary>
        ///     Return true if need create argument Modified in "Get" methods
        /// </summary>
        public bool IsModifiedExist
        {
            get
            {
                var specialOption = "Modified";

                if (JoinRepositoryInfo != null)
                {
                    return JoinRepositoryInfo.Elements.Exists(s => s.Name == specialOption);
                }

                return Elements.Exists(s => s.Name == specialOption);
            }
        }

        public bool IsSyncStateExists
        {
            get
            {
                var specialOption = "SyncState";

                var isSyncStateFieldExists = JoinRepositoryInfo != null
                    ? JoinRepositoryInfo.Elements.Exists(s => s.Name == specialOption)
                    : Elements.Exists(s => s.Name == specialOption);

                return isSyncStateFieldExists;
            }
        }

        /// <summary>
        ///     List of method implementation info
        /// </summary>
        public List<MethodImplementationInfo> MethodImplementationInfo { get; set; }

        /// <summary>
        ///     List of cache repository method implementation info
        /// </summary>
        public List<MethodImplementationInfo> CacheRepositoryMethodImplementationInfo { get; set; }

        /// <summary>
        ///     Info about relation many to many 
        /// </summary>
        public List<Many2ManyInfo> Many2ManyInfo { get; set; }

        /// <summary>
        ///     List of methods name in repository interface
        /// </summary>
        public List<MethodInfo> InterfaceMethods { get; set; }

        /// <summary>
        ///      List of methods name in castom repository class
        /// </summary>
        public List<MethodInfo> CustomRepositoryMethodNames { get; set; }

        /// <summary>
        ///      List of methods name in castom cache repository class (for versioned repository)
        /// </summary>
        public List<MethodInfo> CustomCacheRepositoryMethodNames { get; set; }

        public DatabaseType DatabaseType { get; set; }


        /// <summary>
        ///     Return list of filters key for key based methods
        /// </summary>
        public List<FilterInfo> PossibleKeysForMethods
        {
            get
            {
                // Methods by keys from model 
                var possibleKeyMethods = new List<FilterInfo>();
                // Primary key(s)
                if (!string.IsNullOrEmpty(PrimaryKeyName))
                {
                    possibleKeyMethods.Add(new FilterInfo(PrimaryKeyName, PrimaryKeys, FilterType.PrimaryKey));
                }
                // Version key
                if (!string.IsNullOrEmpty(VersionKeyName))
                {
                    possibleKeyMethods.Add(new FilterInfo(VersionKeyName, new List<ParameterInfo>{ VersionKey}, FilterType.VersionKey));
                }
                // Filter keys
                if (FilterInfos.Any())
                {
                    var filterInfos = FilterInfos;
                    if (!string.IsNullOrEmpty(PrimaryKeyName))
                    {
                        filterInfos = filterInfos.Where(f => f.Key != PrimaryKeyName).ToList();
                    }

                    if (!string.IsNullOrEmpty(VersionKeyName))
                    {
                        filterInfos = filterInfos.Where(f => f.Key != VersionKeyName).ToList();
                    }
                    possibleKeyMethods.AddRange(filterInfos);
                }
                
                return possibleKeyMethods;
            }
        }

        /// <summary>
        ///     Get general repository info for sql script generation 
        /// </summary>
        public SqlInfo RepositorySqlInfo
        {
            get
            {
                // Common info for generate sql scriptes
                var sqlInfo = new SqlInfo
                {
                    TableColumns = Elements.ToList(),
                    HiddenTableColumns = new List<PropertyInfo>(),
                    UpdateTableColumns = Elements.Where(FilterElemsForUpdate).ToList(),
                    TableName = TableName,
                    PrimaryKeyNames = PrimaryKeys.Select(k => k.Name).ToList(),
                    TenantRelated = IsTenantRelated,
                    ReturnPrimaryKey = PrimaryKeys.Count == 1,
                    VersionKeyName = VersionKeyName,
                    VersionKeyType = VersionKey?.TypeName,
                    VersionTableName = VersionTableName,
                    IsManyToMany = IsManyToMany,
                    IdentityColumns = new List<string>(),
                    IdentityColumnsJoined = new List<string>(),
                    IsStoreDependent = IsStoreDependent
                };

                if (JoinRepositoryInfo != null)
                {
                    sqlInfo.JoinTableColumns = JoinRepositoryInfo.Elements.ToList();
                    sqlInfo.JoinTableName = JoinRepositoryInfo.TableName;
                    sqlInfo.JoinPrimaryKeyNames = JoinRepositoryInfo.PrimaryKeys.Select(k => k.Name).ToList();
                    sqlInfo.JoinVersionTableName = VersionTableName != null ? JoinRepositoryInfo.VersionTableName : null;
                    sqlInfo.JoinVersionKeyName = JoinRepositoryInfo.VersionKeyName;
                    sqlInfo.JoinIdentity = JoinRepositoryInfo.Identity;

                    if (sqlInfo.JoinPrimaryKeyNames.Count == 1 && sqlInfo.JoinIdentity)
                    {
                        sqlInfo.IdentityColumnsJoined.Add(sqlInfo.JoinPrimaryKeyNames[0]);
                    }
                }
                var pk = PrimaryKeys.FirstOrDefault();
                sqlInfo.PrimaryKeyType = pk?.TypeName;
                sqlInfo.Identity = Identity;
                sqlInfo.IsDeleted = IsDeletedExist;
                sqlInfo.IsSyncStateEnabled = HasSyncState;
                sqlInfo.IsSyncStateFiledExists = IsSyncStateExists && !HasSyncState;

                if (PrimaryKeys.Count == 1 && Identity)
                {
                    sqlInfo.IdentityColumns.Add(PrimaryKeys[0].Name);
                }

                sqlInfo.HiddenTableColumns = HiddenElements.Select(e => new PropertyInfo(e.Name)).ToList();

                return sqlInfo;
            }
        }

        /// <summary>
        ///     Full type of data access service
        /// </summary>
        public string DataAccessServiceTypeName { get; set; }

        /// <summary>
        ///     Full type of data access controller
        /// </summary>
        public string DataAccessControllerTypeName { get; set; }

        /// <summary>
        ///     Full type of date time service
        /// </summary>
        public string DateTimeServiceTypeName { get; set; }

        /// <summary>
        ///     Full type of base repository time service
        /// </summary>
        public string RepositoryBaseTypeName { get; set; }

        #endregion

        #region Analysis Error

        /// <summary>
        ///    Errors arising in the analysis of repository models
        /// </summary>
        public List<AnalysisError> RepositoryAnalysisError { get; set; }

        public bool RepositoryInterfaceNotFound
        {
            get { return RepositoryAnalysisError.Any(e => e.Error == Infrastructure.RepositoryAnalysisError.InterfaceNotFound); }
        }


        #endregion

        private bool FilterElemsForUpdate(PropertyInfo prop)
        {
            var res = true;

            switch (prop.Name)
            {
                case _syncStateFiledName:
                    res = !HasSyncState;
                    break;
                default:
                    break;
            }

            return res;
        }
    }
}