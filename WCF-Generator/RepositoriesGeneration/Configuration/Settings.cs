﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.Common;

namespace WCFGenerator.RepositoriesGeneration.Configuration
{
    public class RepositoryGeneratorSettings : CommonSettings<RepositoryGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "repositoryGenerator";

        public IEnumerable<RepositoryProject> GetConfigs()
        {
            var section = ConfigurationManager.GetSection(ConfigSectionName) as RepositoryGenerator;
            return section?.RepositoryProjects.Cast<RepositoryProject>() ?? Enumerable.Empty<RepositoryProject>();
        }
    }

    /// <summary>
    ///     Configuration section "RepositoryGenerator"
    /// </summary>
    public class RepositoryGenerator : BasicConfigurationSection
    {
        /// <summary>
        ///     All configured repository RepositoryProjects
        /// </summary>
        [ConfigurationProperty("repositoryProjects")]
        public RepositoryProjects RepositoryProjects
        {
            get { return ((RepositoryProjects)(base["repositoryProjects"])); }
        }
    }

    /// <summary>
    ///     RepositoryProjects collection element
    /// </summary>
    [ConfigurationCollection(typeof(RepositoryProject))]
    public class RepositoryProjects : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RepositoryProject();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RepositoryProject)(element)).Name;
        }

        public RepositoryProject this[int idx]
        {
            get { return (RepositoryProject)BaseGet(idx); }
        }
    }

    /// <summary>
    ///     Task element
    /// </summary>
    public class RepositoryProject : ConfigurationElement
    {
        /// <summary>
        ///     Name
        /// </summary>
        [ConfigurationProperty("Name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return ((string)(base["Name"])); }
        }

        /// <summary>
        ///     Repository Attribute Name
        /// </summary>
        [ConfigurationProperty("RepositoryAttributeName", DefaultValue = "", IsRequired = true)]
        public string RepositoryAttributeName
        {
            get { return ((string)(base["RepositoryAttributeName"])); }
        }

        /// <summary>
        ///     Repository base type name
        /// </summary>
        [ConfigurationProperty("RepositoryBase", DefaultValue = "", IsRequired = false)]
        public string RepositoryBase
        {
            get { return ((string)(base["RepositoryBase"])); }
        }

        /// <summary>
        ///     Target Project Name for save file
        /// </summary>
        [ConfigurationProperty("TargetProjectName", DefaultValue = "",  IsRequired = true)]
        public string TargetProjectName
        {
            get { return ((string)(base["TargetProjectName"])); }
        }

        /// <summary>
        ///      Name of target folder in project
        /// </summary>
        [ConfigurationProperty("RepositoryTargetFolder", DefaultValue = "", IsRequired = true)]
        public string RepositoryTargetFolder
        {
            get { return ((string)(base["RepositoryTargetFolder"])); }
        }

        /// <summary>
        ///     Standard suffix repository
        /// </summary>
        [ConfigurationProperty("RepositorySuffix", DefaultValue = "",  IsRequired = true)]
        public string RepositorySuffix
        {
            get { return ((string)(base["RepositorySuffix"])); }
        }

        /// <summary>
        ///      Name of project which contains interfaces
        /// </summary>
        [ConfigurationProperty("RepositoryInterfacesProjectName", DefaultValue = "", IsRequired = true)]
        public string RepositoryInterfacesProjectName
        {
            get { return ((string)(base["RepositoryInterfacesProjectName"])); }
        }

        /// <summary>
        ///      Project name which include repository models
        /// </summary>
        [ConfigurationProperty("RepositoryClassProjects", DefaultValue = "",  IsRequired = true)]
        public string RepositoryClassProjects
        {
            get { return ((string)(base["RepositoryClassProjects"])); }
        }

        /// <summary>
        ///      List of project for analysis
        /// </summary>
        [ConfigurationProperty("AdditionalProjects", DefaultValue = "", IsRequired = false)]
        public string AdditionalProjects
        {
            get { return ((string)(base["AdditionalProjects"])); }
        }

        /// <summary>
        ///     Default namespace
        /// </summary>
        [ConfigurationProperty("DefaultNamespace", DefaultValue = "", IsRequired = true)]
        public string DefaultNamespace
        {
            get { return ((string)(base["DefaultNamespace"])); }
        }

        /// <summary>
        ///     Database type
        /// </summary>
        [ConfigurationProperty("DatabaseType", DefaultValue = 1, IsRequired = true)]
        public int DatabaseType
        {
            get { return ((int)(base["DatabaseType"])); }
        }
    }
}
