using System;
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
            Keys = new List<string>();
            FilterKeys = new List<ParameterInfo>();
            FilterInfos = new List<FilterInfo>();
            PrimaryKeys = new List<ParameterInfo>();
            Elements = new List<string>();
            JoinedElements = new List<string>();
        }

        public string RepositorySuffix { get; set; }

        #region Repository model class info

        // Info

        /// <summary>
        ///     Repository model type
        /// </summary>
        public string ClassName { get; set; }

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


        public FilterOption FilterData { get; set; }
        public List<string> Keys { get; set; }

        public string GenericRepositoryName { get; set; }

        public List<string> PrimaryKeyNames { get; set; }// TODO delete

        /// <summary>
        ///     Primary Keys
        /// </summary>
        public List<ParameterInfo> PrimaryKeys { get; set; }
        public string PrimaryKeyName { get { return string.Join("And", PrimaryKeys.Select(info => info.Name)); } }

        public List<ParameterInfo> FilterKeys { get; set; } // TODO refactor

        /// <summary>
        ///     Filters
        /// </summary>
        public List<FilterInfo> FilterInfos { get; set; }

        /// <summary>
        ///     Special filter options
        /// </summary>
        public FilterInfo SpecialOptions { get; set; }

        public string RepositoriesNamespace { get; set; }
        public string VersionKey { get; set; }

        // Flags
        public bool IsVersioning { get; set; }
        public bool IsTenantRelated { get; set; }
        public bool IsConstructorImplemented { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsNewKey { get; set; }
        public bool IsFilterDataGeneration { get; set; }

        #endregion

        #region Joined repository class info (inherited base class)

        // Info
        public string JoinedClassName { get; set; }
        public string JoinedFullClassName { get; set; }
        public string TableNameJoined { get; set; }
        public List<string> JoinedElements { get; set; }
        public string PrimaryKeyJoined { get; set; }
        public string VersionKeyJoined { get; set; }
        public FilterOption FilterDataJoined { get; set; }

        // Flags
        public bool IsJoined { get { return JoinedClassName != null; }}
        public bool IsIdentityJoined { get; set; }

        #endregion

        #region Syntax info

        /// <summary>
        ///     Class declaration syntax of data object class from which generate repository class
        /// </summary>
        public ClassDeclarationSyntax DOClass { get; set; }

        /// <summary>
        ///     Class declaration syntax of joined data object class from which generate repository class
        /// </summary>
        public ClassDeclarationSyntax JoinedClass { get; set; }

        /// <summary>
        ///     Interface declaration syntax for interface of repository class which implemented interface "IRepository{T}"
        /// </summary>
        public InterfaceDeclarationSyntax RepositoryInterface { get; set; }

        /// <summary>
        ///     List of method declaration syntax for methods in repository interface
        /// </summary>
        public IEnumerable<MethodDeclarationSyntax> RepositoryInterfaceMethods { get; set; }

        /// <summary>
        ///     Class declaration syntax of partial repository class which implemented interface "IRepository{T}"
        /// </summary>
        public ClassDeclarationSyntax CustomRepository { get; set; }

        /// <summary>
        ///     List of method declaration syntax for methods in custom part of partial repository class
        /// </summary>
        public List<MethodDeclarationSyntax> CustomRepositoryMethods { get; set; }

        /// <summary>
        ///     List of method implementation info
        /// </summary>
        public IEnumerable<MethodImplementationInfo> MethodImplementationInfo { get; set; }

        #endregion

        public List<FilterInfo> GetPossibleKeysForMethods()
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


    internal class FilterInfo
    {
        public FilterInfo(string key, List<ParameterInfo> parameters)
        {
            Key = key;
            Parameters = parameters;
        }

        public string Key { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
    }
}