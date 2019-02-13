using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.Common;
using WCFGenerator.Common.ApiDecoration;

namespace WCFGenerator.CustomerApiDecoratorsGeneration.Configuration
{
    public class CustomerApiDecoratorsSettings : CommonSettings<CustomerApiDecoratorsSettings>
    {
        protected override string ConfigSectionName { get; } = "customerApiDecoratorsGenerator";

        public IReadOnlyCollection<ApiDecoratorsConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as CustomerApiDecoratorsGenerator;
                return section?.CustomerApiDecoratorsConfigurations.Cast<ApiDecoratorsConfiguration>().ToList();
            }
            catch (ConfigurationErrorsException e)
            {
                throw new InvalidOperationException("Error occurred on trying get Wcf Customer Generator settings.", e);
            }
        }
    }

    public class CustomerApiDecoratorsGenerator : BasicConfigurationSection
    {
        [ConfigurationProperty("customerApiDecoratorsConfigurations")]
        public CustomerApiDecoratorsConfigurationColection CustomerApiDecoratorsConfigurations
        {
            get { return ((CustomerApiDecoratorsConfigurationColection) (base["customerApiDecoratorsConfigurations"])); }
        }
    }

    [ConfigurationCollection(typeof(ApiDecoratorsConfiguration))]
    public class CustomerApiDecoratorsConfigurationColection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApiDecoratorsConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApiDecoratorsConfiguration) (element)).SourceInterface;
        }

        public ApiDecoratorsConfiguration this[int idx]
        {
            get { return (ApiDecoratorsConfiguration) BaseGet(idx); }
        }
    }
}