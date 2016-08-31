using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionedRepositoryGeneration.Generator.Heplers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal class RepositoryInfo
    {
        public RepositoryInfo()
        {
            FilterInfos = new List<FilterInfo>();
            PrimaryKeys = new List<ParameterInfo>();
            Elements = new List<string>();
            Many2ManyInfo = new List<Many2ManyInfo>();
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
        ///     Repository model type as method parameter name
        /// </summary>
        public string ParameterName { get { return ClassName.FirstSymbolToLower(); } }

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
        ///     Returns the name of generic repository interface
        /// </summary>
        public string GenericRepositoryName { get { return string.Format("I{0}<{1}>", RepositorySuffix, ClassName); } }

        /// <summary>
        ///     Name of repository interface
        /// </summary>
        public string RepositoryInterfaceName { get; set; }

        /// <summary>
        ///     Primary Keys
        /// </summary>
        public List<ParameterInfo> PrimaryKeys { get; set; }

        /// <summary>
        ///     Returns the name of the combined primary key 
        /// </summary>
        public string PrimaryKeyName { get { return string.Join("And", PrimaryKeys.Select(info => info.Name)); } }

        /// <summary>
        ///     Filters
        /// </summary>
        public List<FilterInfo> FilterInfos { get; set; }

        /// <summary>
        ///     Special filter options (it is "isDeleted")
        /// </summary>
        public FilterInfo SpecialOptions { get; set; }

        /// <summary>
        ///     Name of member of repository model which marked as version key
        /// </summary>
        public string VersionKey { get; set; }

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
        ///     List of method implementation info
        /// </summary>
        public IEnumerable<MethodImplementationInfo> MethodImplementationInfo { get; set; }

        /// <summary>
        ///     Info about relation many to many 
        /// </summary>
        public List<Many2ManyInfo> Many2ManyInfo { get; set; }

        /// <summary>
        ///     Return list of filters key for key based methods
        /// </summary>
        public List<FilterInfo> PossibleKeysForMethods
        {
            get
            {
                // Methods by keys from model (without methods from base model)
                var possibleKeyMethods = new List<FilterInfo>();
                // Primary key(s)
                if (!string.IsNullOrEmpty(PrimaryKeyName))
                {
                    possibleKeyMethods.Add(new FilterInfo(PrimaryKeyName, PrimaryKeys));
                }
                // Filter keys
                if (FilterInfos.Any())
                {
                    possibleKeyMethods.AddRange(FilterInfos);
                }
                return possibleKeyMethods;
            }
        }

        #endregion
    }
}