using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;

namespace WCFGenerator.MappingsGenerator.Configuration
{
    public class MappingGeneratorSettings : CommonSettings<MappingGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "mappingGenerator";

        public IReadOnlyCollection<MappingConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as MappingGenerator;
                return section?.MappingConfigurations.Cast<MappingConfiguration>().ToList();
            }
            catch (ConfigurationErrorsException e)
            {
                throw new InvalidOperationException("Error occurred on trying get Wcf Client Generator settings.", e);
            }
        }
    }

    public class MappingGenerator : BasicConfigurationSection
    {
        /// <summary>
        ///     All configured wcf services for generate client
        /// </summary>
        [ConfigurationProperty("mappingConfigurations")]
        public MappingConfigurationsCollection MappingConfigurations
        {
            get { return ((MappingConfigurationsCollection)(base["mappingConfigurations"])); }
        }
    }

    [ConfigurationCollection(typeof(MappingConfiguration))]
    public class MappingConfigurationsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingConfiguration)(element)).MapExtensionNameSpace;
        }

        public MappingConfiguration this[int idx]
        {
            get { return (MappingConfiguration)BaseGet(idx); }
        }
    }

    public class MappingConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("MapExtensionNameSpace", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string MapExtensionNameSpace //= "YumaPos.Server.BackOffice.Presentation.Generation";
        {
            get { return ((string)(base["MapExtensionNameSpace"])); }
        }

        [ConfigurationProperty("MapExtensionClassName", DefaultValue = "MapExtensions", IsRequired = false)]
        public string MapExtensionClassName
        {
            get { return ((string)(base["MapExtensionClassName"])); }
        }

        [ConfigurationProperty("MapAttribute", DefaultValue = "Map", IsRequired = false)]
        public string MapAttribute
        {
            get { return ((string)(base["MapAttribute"])); }
        }
        [ConfigurationProperty("MapIgnoreAttribute", DefaultValue = "MapIgnore", IsRequired = false)]
        public string MapIgnoreAttribute
        {
            get { return ((string)(base["MapIgnoreAttribute"])); }
        }
        [ConfigurationProperty("DtoSuffix", DefaultValue = "Dto", IsRequired = false)]
        public string DtoSuffix
        {
            get { return ((string)(base["DtoSuffix"])); }
        }
    }
}
