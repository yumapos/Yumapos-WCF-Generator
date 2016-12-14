using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.DecoratorGeneration.Configuration
{
    public static class DecoratorGeneratorSettings
    {
        public static IReadOnlyCollection<DecoratorConfiguration> GetConfigs()
        {
            var section = ConfigurationManager.GetSection("decoratorGenerator") as DecoratorGenerator;
            if(section==null) throw new ConfigurationErrorsException("decoratorGenerator configuration section not found");
            return section.RepositoryProjects.Cast<DecoratorProject>().Select(s=> new DecoratorConfiguration()
            {
                SolutionProjectName = s.Name,
                DecoratedClassNames = s.DecoratedClasses.Cast<DecoratedClass>().Select(c=>c.FullTypeName).ToList()
            }).ToList();
        }
    }

    public class DecoratorConfiguration
    {
        public string SolutionProjectName { get; set; }
        public List<string> DecoratedClassNames { get; set; }
    }

    /// <summary>
    ///     Configuration section "DecoratorGenerator"
    /// </summary>
    public class DecoratorGenerator : ConfigurationSection
    {
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
    [ConfigurationCollection(typeof(DecoratorProject), AddItemName = "project")]
    public class DecoratorProjects : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DecoratorProject();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DecoratorProject)(element)).Name;
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
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = false)]
        public string Name
        {
            get { return ((string)(base["name"])); }
        }

        /// <summary>
        ///     All classes
        /// </summary>
        [ConfigurationProperty("decoratedClasses")]
        public DecoratedClasses DecoratedClasses
        {
            get { return ((DecoratedClasses)(base["decoratedClasses"])); }
        }
    }

    /// <summary>
    ///     Collection of element
    /// </summary>
    [ConfigurationCollection(typeof(DecoratedClass))]
    public class DecoratedClasses : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DecoratedClass();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DecoratedClass)(element)).FullTypeName;
        }

        public DecoratedClass this[int idx]
        {
            get { return (DecoratedClass)BaseGet(idx); }
        }
    }

    /// <summary>
    ///     Decorator Class
    /// </summary>
    public class DecoratedClass : ConfigurationElement
    {
        /// <summary>
        ///      Types for analysis
        /// </summary>
        [ConfigurationProperty("fullTypeName", DefaultValue = "", IsRequired = true)]
        public string FullTypeName
        {
            get { return ((string)(base["fullTypeName"])); }
        }
    }
}
