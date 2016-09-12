using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionedRepositoryGeneration.Generator.Heplers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal class RepositoryInfo
    {
        private string _versionKey;
        private string _primaryKeyName;
        private List<ParameterInfo> _primaryKeys;
        private bool _identity;

        public RepositoryInfo()
        {
            FilterInfos = new List<FilterInfo>();
            PrimaryKeys = new List<ParameterInfo>();
            Elements = new List<string>();
            Many2ManyInfo = new List<Many2ManyInfo>();
            InterfaceMethodNames = new List<string>();
            CustomRepositoryMethodNames = new List<string>();
            CustomCacheRepositoryMethodNames = new List<string>();
            MethodImplementationInfo = new List<MethodImplementationInfo>();
            CacheRepositoryMethodImplementationInfo = new List<MethodImplementationInfo>();
        }

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
        ///     Property names - table columns
        /// </summary>
        public List<string> Elements { get; set; }

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
        ///     Filters
        /// </summary>
        public List<FilterInfo> FilterInfos { get; set; }

        /// <summary>
        ///     Special filter options (it is "isDeleted")
        /// </summary>
        public FilterInfo SpecialOptions { get; set; }

        /// <summary>
        ///     Name of member of repository model which marked as version key (for versioned repository)
        /// </summary>
        public string VersionKey
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
        ///     Repository joined to another repository and can not generate
        /// </summary>
        public bool IsJoned { get; set; }

        /// <summary>
        ///     Repository is tenant related
        /// </summary>
        public bool IsTenantRelated { get; set; }

        /// <summary>
        ///     Return true if constructor implemented in custom repository 
        /// </summary>
        public bool IsConstructorImplemented { get; set; }

        /// <summary>
        ///     Return true if constructor implemented in custom cache repository (for versioned repository)
        /// </summary>
        public bool IsCacheRepositoryConstructorImplemented { get; set; }

        /// <summary>
        ///     Return true if need create argument IsDeleted in "Get" methods
        /// </summary>
        public bool IsDeletedExist
        {
            get
            {
                var specialOption = SpecialOptions.Parameters.First().Name;

                if (JoinRepositoryInfo != null)
                {
                    return JoinRepositoryInfo.Elements.Exists(s => s == specialOption);
                }

                return Elements.Exists(s => s == specialOption);
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
        public List<string> InterfaceMethodNames { get; set; }

        /// <summary>
        ///      List of methods name in castom repository class
        /// </summary>
        public List<string> CustomRepositoryMethodNames { get; set; }

        /// <summary>
        ///      List of methods name in castom cache repository class (for versioned repository)
        /// </summary>
        public List<string> CustomCacheRepositoryMethodNames { get; set; }

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
                // Filter keys
                if (FilterInfos.Any())
                {
                    possibleKeyMethods.AddRange(FilterInfos);
                }
                
                return possibleKeyMethods;
            }
        }

        public bool Identity { get; set; }

        #endregion
    } 
}