using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.DecoratorGeneration.Configuration
{
    public static class DecoratorGeneratorSettings
    {
        public static IEnumerable<DecoratorProject> GetConfigs()
        {
            var section = ConfigurationManager.GetSection("decoratorGenerator") as DecoratorGenerator;
            return section.RepositoryProjects.Cast<DecoratorProject>();
        }

        public static bool GenerationEnabled
        {
            get
            {
                var section = ConfigurationManager.GetSection("decoratorGenerator") as DecoratorGenerator;
                return section.CommonSettings.Enable;
            }
        }
    }

    /// <summary>
    ///     Configuration section "DecoratorGenerator"
    /// </summary>
    public class DecoratorGenerator : ConfigurationSection
    {
        /// <summary>
        ///    Common Settings
        /// </summary>
        [ConfigurationProperty("commonSettings")]
        public CommonSettings CommonSettings
        {
            get { return ((CommonSettings)(base["commonSettings"])); }
        }
        /// <summary>
        ///     All projects where search class for decorate
        /// </summary>
        [ConfigurationProperty("decoratorProjects")]
        public DecoratorProjects RepositoryProjects
        {
            get { return ((DecoratorProjects)(base["decoratorProjects"])); }
        }
    }

    /// <summary>
    ///     Collection of element
    /// </summary>
    [ConfigurationCollection(typeof(DecoratorProject))]
    public class DecoratorProjects : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DecoratorProject();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DecoratorProject)(element)).Project;
        }

        public DecoratorProject this[int idx]
        {
            get { return (DecoratorProject)BaseGet(idx); }
        }
    }

    /// <summary>
    ///     Project element
    /// </summary>
    public class DecoratorProject : ConfigurationElement
    {
        /// <summary>
        ///      Project for analysis
        /// </summary>
        [ConfigurationProperty("Project", DefaultValue = "", IsRequired = false)]
        public string Project
        {
            get { return ((string)(base["Project"])); }
        }
    }

    /// <summary>
    ///     Common Settings
    /// </summary>
    public class CommonSettings : ConfigurationElement
    {
        [ConfigurationProperty("Enable", DefaultValue = "false", IsKey = true, IsRequired = true)]
        public bool Enable
        {
            get { return ((bool)(base["Enable"])); }
        }
    }
}
