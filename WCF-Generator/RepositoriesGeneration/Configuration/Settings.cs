using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.Common;
using WCFGenerator.RepositoriesGeneration.Enums;

namespace WCFGenerator.RepositoriesGeneration.Configuration
{
    public class RepositoryGeneratorSettings : CommonSettings<RepositoryGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "repositoryGenerator";

        public IEnumerable<RepositoryProject> GetConfigs()
        {
            var section = Program.GlobalConfig.GetSection(ConfigSectionName) as RepositoryGenerator;
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
            get { return ((string)(base[nameof(Name)])); }
        }

        /// <summary>
        ///     Repository Attribute Name
        /// </summary>
        [ConfigurationProperty("RepositoryAttributeName", DefaultValue = "", IsRequired = true)]
        public string RepositoryAttributeName
        {
            get { return ((string)(base[nameof(RepositoryAttributeName)])); }
        }

        /// <summary>
        ///     Repository base type name
        /// </summary>
        [ConfigurationProperty("RepositoryBase", DefaultValue = "", IsRequired = false)]
        public string RepositoryBase
        {
            get { return ((string)(base[nameof(RepositoryBase)])); }
        }

        /// <summary>
        ///     Target Project Name for save file
        /// </summary>
        [ConfigurationProperty("TargetProjectName", DefaultValue = "",  IsRequired = true)]
        public string TargetProjectName
        {
            get { return ((string)(base[nameof(TargetProjectName)])); }
        }

        /// <summary>
        ///      Name of target folder in project
        /// </summary>
        [ConfigurationProperty("RepositoryTargetFolder", DefaultValue = "", IsRequired = true)]
        public string RepositoryTargetFolder
        {
            get { return ((string)(base[nameof(RepositoryTargetFolder)])); }
        }

        /// <summary>
        ///     Standard suffix repository
        /// </summary>
        [ConfigurationProperty("RepositorySuffix", DefaultValue = "",  IsRequired = true)]
        public string RepositorySuffix
        {
            get { return ((string)(base[nameof(RepositorySuffix)])); }
        }

        /// <summary>
        ///      Name of project which contains interfaces
        /// </summary>
        [ConfigurationProperty("RepositoryInterfacesProjectName", DefaultValue = "", IsRequired = true)]
        public string RepositoryInterfacesProjectName
        {
            get { return ((string)(base[nameof(RepositoryInterfacesProjectName)])); }
        }

        /// <summary>
        ///      Project name which include repository models
        /// </summary>
        [ConfigurationProperty("RepositoryClassProjects", DefaultValue = "",  IsRequired = true)]
        public string RepositoryClassProjects
        {
            get { return ((string)(base[nameof(RepositoryClassProjects)])); }
        }

        /// <summary>
        ///      List of project for analysis
        /// </summary>
        [ConfigurationProperty("AdditionalProjects", DefaultValue = "", IsRequired = false)]
        public string AdditionalProjects
        {
            get { return ((string)(base[nameof(AdditionalProjects)])); }
        }

        /// <summary>
        ///     Default namespace
        /// </summary>
        [ConfigurationProperty("DefaultNamespace", DefaultValue = "", IsRequired = true)]
        public string DefaultNamespace
        {
            get { return ((string)(base[nameof(DefaultNamespace)])); }
        }

        /// <summary>
        ///     Database type
        /// </summary>
        [ConfigurationProperty("DatabaseType", DefaultValue = 1, IsRequired = true)]
        public int DatabaseType
        {
            get { return ((int)(base[nameof(DatabaseType)])); }
        }
        
        [ConfigurationProperty("InsertManyMethod", DefaultValue = Enums.InsertManyMethod.ViaRows, IsRequired = false)]
        public InsertManyMethod InsertManyMethod
        {
            get { return ((InsertManyMethod)(base[nameof(InsertManyMethod)])); }
        }
        
        /// <summary>
        ///     Target Migration Project Name for save DataTable Migrations
        /// </summary>
        [ConfigurationProperty("DataTableMigrationProjectName", DefaultValue = "",  IsRequired = false)]
        public string DataTableMigrationProjectName
        {
            get { return ((string)(base[nameof(DataTableMigrationProjectName)])); }
        }
        /// <summary>
        ///      Name of target folder in Migration project
        /// </summary>
        [ConfigurationProperty("DataTableMigrationTargetFolder", DefaultValue = "", IsRequired = false)]
        public string DataTableMigrationTargetFolder
        {
            get { return ((string)(base[nameof(DataTableMigrationTargetFolder)])); }
        }
        
        /// <summary>
        ///      Name of namespace in Migration project
        /// </summary>
        [ConfigurationProperty("DataTableMigrationNamespace", DefaultValue = "", IsRequired = false)]
        public string DataTableMigrationNamespace
        {
            get { return ((string)(base[nameof(DataTableMigrationNamespace)])); }
        }
        
        /// <summary>
        ///      Force recreate migrations
        /// </summary>
        [ConfigurationProperty("ForceRecreateMigrations", DefaultValue = false, IsRequired = false)]
        public bool ForceRecreateMigrations
        {
            get { return ((bool)(base[nameof(ForceRecreateMigrations)])); }
        }
    }
}
