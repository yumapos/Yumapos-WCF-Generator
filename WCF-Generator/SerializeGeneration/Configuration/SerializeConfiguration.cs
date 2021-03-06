﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.Common;
using WCFGenerator.Common.Configuration;

namespace WCFGenerator.SerializeGeneration.Configuration
{
    public class SerializeGeneratorSettings : CommonSettings<SerializeGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "serialize";

        public SerializeConfiguration GetConfig()
        {
            var section = ConfigurationManager.GetSection(ConfigSectionName) as SerializeConfiguration;
            return section;
        }

        public IEnumerable<string> InvalidPropertyTypes
        {
            get { return GetConfig().InvalidPropertyTypes.Cast<TypeElement>().Select(t => t.Type); }
        }
    }

    public class SerializeConfiguration : BasicConfigurationSection
    {
        [ConfigurationProperty("baseInterface", IsRequired = true)]
        public string BaseInterface => this["baseInterface"].ToString();


        [ConfigurationProperty("ignoreAttribute", IsRequired = true)]
        public string IgnoreAttribute => this["ignoreAttribute"].ToString();


        [ConfigurationProperty("includeAttribute", IsRequired = true)]
        public string IncludeAtribute => this["includeAttribute"].ToString();


        [ConfigurationProperty("invalidPropertyTypes", IsRequired = true)]
        public TypeElementCollection InvalidPropertyTypes => (TypeElementCollection)base["invalidPropertyTypes"];

        [ConfigurationProperty("projectNames", IsRequired = true)]
        public GenerationProjectCollection ProjectNames => (GenerationProjectCollection)base["projectNames"];

        [ConfigurationProperty("helpProjectNames", IsRequired = false)]
        public MappingProjectCollection HelpProjectNames => (MappingProjectCollection)base["helpProjectNames"];

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
        public string MappingAttribute => this["mappingAttribute"].ToString();


        [ConfigurationProperty("generationPrefix", IsRequired = true)]
        public string GenerationPrefix => this["generationPrefix"].ToString();


        [ConfigurationProperty("mappingIgnoreAttribute", IsRequired = true)]
        public string MappingIgnoreAttribute => this["mappingIgnoreAttribute"].ToString();

        [ConfigurationProperty("migrationProject", IsRequired = true)] 
        public string MigrationProject => this["migrationProject"].ToString();

        [ConfigurationProperty("migrationVersionClass", IsRequired = true)]
        public string MigrationVersionClass => this["migrationVersionClass"].ToString();

        [ConfigurationProperty("migrationInterface", IsRequired = true)]
        public string MigrationInterface => this["migrationInterface"].ToString();

        [ConfigurationProperty("migrationClassPrefix", IsRequired = true)]
        public string MigrationClassPrefix => this["migrationClassPrefix"].ToString();

        [ConfigurationProperty("migrationIgnoreAttribute", IsRequired = true)]
        public string MigrationIgnoreAttribute => this["migrationIgnoreAttribute"].ToString();

        [ConfigurationProperty("migrationVersionProject", IsRequired = true)]
        public string MigrationVersionProject => this["migrationVersionProject"].ToString();
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
