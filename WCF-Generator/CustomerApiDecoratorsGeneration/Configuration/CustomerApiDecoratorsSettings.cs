using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WCFGenerator.ClientApiDecoratorsGeneration.Configuration;
using WCFGenerator.Common;

namespace WCFGenerator.CustomerApiDecoratorsGeneration.Configuration
{
    public class CustomerApiDecoratorsSettings : CommonSettings<CustomerApiDecoratorsSettings>
    {
        protected override string ConfigSectionName { get; } = "customerApiDecoratorsGenerator";

        public IReadOnlyCollection<CustomerApiDecoratorsConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as CustomerApiDecoratorsGenerator;
                return section?.CustomerApiDecoratorsConfigurations.Cast<CustomerApiDecoratorsConfiguration>().ToList();
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

    [ConfigurationCollection(typeof(CustomerApiDecoratorsConfiguration))]
    public class CustomerApiDecoratorsConfigurationColection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomerApiDecoratorsConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomerApiDecoratorsConfiguration) (element)).SourceInterface;
        }

        public CustomerApiDecoratorsConfiguration this[int idx]
        {
            get { return (CustomerApiDecoratorsConfiguration) BaseGet(idx); }
        }
    }

    public class CustomerApiDecoratorsConfiguration : ConfigurationElement
    {

        [ConfigurationProperty("sourceInterface", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string SourceInterface
        {
            get { return ((string)(base["sourceInterface"])); }
        }

        [ConfigurationProperty("sourceProject", DefaultValue = "", IsRequired = true)]
        public string SourceProject
        {
            get { return ((string)(base["sourceProject"])); }
        }

        [ConfigurationProperty("targetProject", DefaultValue = "", IsRequired = true)]
        public string TargetProject
        {
            get { return ((string)(base["targetProject"])); }
        }

        [ConfigurationProperty("targetFolder", DefaultValue = "", IsRequired = true)]
        public string TargetFolder
        {
            get { return ((string)(base["targetFolder"])); }
        }

        [ConfigurationProperty("targetNamespace", DefaultValue = "", IsRequired = true)]
        public string TargetNamespace
        {
            get { return ((string)(base["targetNamespace"])); }
        }

        [ConfigurationProperty("partialClass", DefaultValue = "", IsRequired = false)]
        public string PartialClass
        {
            get { return ((string) (base["partialClass"])); }
        }
    }
}