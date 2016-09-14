using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.SerializeGeneration
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

        public IEnumerable<string> AllProjectNames
        {
            get
            {
                return (from object standartType in ProjectNames select standartType as GenerationProjectElement).Select(x => x.NameProject).ToArray();
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
        public GenerationProjectElement this[int index]
        {
            get
            {
                return BaseGet(index) as GenerationProjectElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new GenerationProjectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GenerationProjectElement)element).NameProject;
        }

        public new GenerationProjectElement this[string key]
        {
            get { return (GenerationProjectElement)BaseGet(key); }
        }
    }
}
