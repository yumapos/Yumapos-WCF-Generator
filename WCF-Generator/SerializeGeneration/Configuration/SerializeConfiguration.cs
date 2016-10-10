using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.SerializeGeneration.Configuration
{
    public class SerializeConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("baseInterface", IsRequired = true)]
        public string BaseInterface
        {
            get
            {
                return this["baseInterface"].ToString();
            }
        }

        [ConfigurationProperty("ignoreAttribute", IsRequired = true)]
        public string IgnoreAttribute
        {
            get
            {
                return this["ignoreAttribute"].ToString();
            }
        }

        [ConfigurationProperty("includeAttribute", IsRequired = true)]
        public string IncludeAtribute
        {
            get
            {
                return this["includeAttribute"].ToString();
            }
        }

        [ConfigurationProperty("projectNames", IsRequired = true)]
        public GenerationProjectCollection ProjectNames
        {
            get
            {
                return (GenerationProjectCollection)base["projectNames"];
            }
        }

        [ConfigurationProperty("helpProjectNames", IsRequired = false)]
        public MappingProjectCollection HelpProjectNames
        {
            get
            {
                return (MappingProjectCollection)base["helpProjectNames"];
            }
        }

        public IEnumerable<string> AllProjectNames
        {
            get
            {
                return (from object standartType in ProjectNames select standartType as GenerationProjectElement).Select(x => x.NameProject).ToArray();
            }
        }

        public IEnumerable<string> AllHelpProjectNames
        {
            get
            {
                return (from object standartType in HelpProjectNames select standartType as MappingProjectElement).Select(x => x.NameProject).ToArray();
            }
        }

        [ConfigurationProperty("mappingAttribute", IsRequired = false)]
        public string MappingAttribute
        {
            get
            {
                return this["mappingAttribute"].ToString();
            }
        }

        [ConfigurationProperty("generationPrefix", IsRequired = true)]
        public string GenerationPrefix
        {
            get
            {
                return this["generationPrefix"].ToString();
            }
        }

        [ConfigurationProperty("mappingIgnoreAttribute", IsRequired = true)]
        public string MappingIgnoreAttribute
        {
            get
            {
                return this["mappingIgnoreAttribute"].ToString();
            }
        }
    }

    public class GenerationProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("generationProjects", IsRequired = true)]
        public string NameProject
        {
            get
            {
                return ((string)(base["generationProjects"]));
            }
            set
            {
                base["generationProjects"] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(GenerationProjectElement))]
    public class GenerationProjectCollection : ConfigurationElementCollection
    {
        public GenerationProjectElement this[int index] => BaseGet(index) as GenerationProjectElement;

        protected override ConfigurationElement CreateNewElement()
        {
            return new GenerationProjectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GenerationProjectElement)element).NameProject;
        }

        public new GenerationProjectElement this[string key] => (GenerationProjectElement)BaseGet(key);
    }

    public class MappingProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("helpProject", IsRequired = true)]
        public string NameProject
        {
            get
            {
                return (string)base["helpProject"];
            }
            set
            {
                base["helpProject"] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(MappingProjectElement))]
    public class MappingProjectCollection : ConfigurationElementCollection
    {
        public MappingProjectElement this[int index] => BaseGet(index) as MappingProjectElement;

        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingProjectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingProjectElement)element).NameProject;
        }

        public new MappingProjectElement this[string key] => (MappingProjectElement)BaseGet(key);
    }
}
