using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Configuration
{
    public class ClientApiDecoratorsGeneratorSettings : CommonSettings<ClientApiDecoratorsGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "clientApiDecoratorsGenerator";

        public IReadOnlyCollection<ApiDecoratorsConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as ClientApiDecoratorsGenerator;
                return section?.ClientApiDecoratorsConfigurations.Cast<ApiDecoratorsConfiguration>().ToList();
            }
            catch (ConfigurationErrorsException e)
            {
                throw new InvalidOperationException("Error occurred on trying get Wcf Client Generator settings.", e);
            }
        }
    }

    public class ClientApiDecoratorsGenerator : BasicConfigurationSection
    {
        [ConfigurationProperty("clientApiDecoratorsConfigurations")]
        public ClientApiDecoratorsConfigurationsCollection ClientApiDecoratorsConfigurations
        {
            get { return ((ClientApiDecoratorsConfigurationsCollection)(base["clientApiDecoratorsConfigurations"])); }
        }
    }

    [ConfigurationCollection(typeof(ApiDecoratorsConfiguration))]
    public class ClientApiDecoratorsConfigurationsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApiDecoratorsConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApiDecoratorsConfiguration)(element)).SourceInterface;
        }

        public ApiDecoratorsConfiguration this[int idx]
        {
            get { return (ApiDecoratorsConfiguration)BaseGet(idx); }
        }
    }
}
