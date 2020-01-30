using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;
using WCFGenerator.Common.Configuration;

namespace WCFGenerator.ResponseDtoGeneration.Configuration
{
    public class ResponseDtoGeneratorSettings : CommonSettings<ResponseDtoGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "responseDtoGenerator";

        public IReadOnlyCollection<ResponseDtoConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as ResponseDtoGenerator;
                return section?.ResponseDtoConfigurations.Cast<ResponseDtoConfiguration>().ToList();
            }
            catch (ConfigurationErrorsException e)
            {
                throw new InvalidOperationException("Error occurred on trying get Wcf Client Generator settings.", e);
            }
        }
    }

    public class ResponseDtoGenerator : BasicConfigurationSection
    {
        [ConfigurationProperty("responseDtoConfigurations")]
        public ResponseDtoConfigurationsCollection ResponseDtoConfigurations
        {
            get { return ((ResponseDtoConfigurationsCollection)(base["responseDtoConfigurations"])); }
        }
    }

    [ConfigurationCollection(typeof(ResponseDtoConfiguration))]
    public class ResponseDtoConfigurationsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ResponseDtoConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ResponseDtoConfiguration)(element)).ProjectForGeneratedCode;
        }

        public ResponseDtoConfiguration this[int idx]
        {
            get { return (ResponseDtoConfiguration)BaseGet(idx); }
        }
    }

    public class ResponseDtoConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("projectForGeneratedCode", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ProjectForGeneratedCode //= "YumaPos.Server.BackOffice.Presentation.Generation";
        {
            get { return ((string)(base["projectForGeneratedCode"])); }
        }

        [ConfigurationProperty("generatedDtosNameSpace", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string MapExtensionNameSpace //= "YumaPos.Server.BackOffice.Presentation.Generation";
        {
            get { return ((string)(base["generatedDtosNameSpace"])); }
        }
        [ConfigurationProperty("prefixStrings")]
        public PrefixStrings PrefixStrings
        {
            get { return ((PrefixStrings)(base["prefixStrings"])); }
        }
    }
}
