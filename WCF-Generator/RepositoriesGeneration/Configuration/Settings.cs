using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.RepositoriesGeneration.Configuration
{
    public static class RepositoryGeneratorSettings
    {
        public static IEnumerable<RepositoryProject> GetConfigs()
        {
            var section = ConfigurationManager.GetSection("repositoryGenerator") as RepositoryGenerator;
            return section.RepositoryProjects.Cast<RepositoryProject>();
        }

        public static string GetSolutionPath()
        {
            var ret = ConfigurationManager.AppSettings["SolutionPath"];
            return ret;
        }
    }

    /// <summary>
    ///     Configuration section "RepositoryGenerator"
    /// </summary>
    public class RepositoryGenerator : ConfigurationSection
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

    }
}
