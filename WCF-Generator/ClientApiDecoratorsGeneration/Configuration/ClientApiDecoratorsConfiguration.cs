using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFGenerator.Common;

namespace WCFGenerator.ClientApiDecoratorsGeneration.Configuration
{
    public class ClientApiDecoratorsGeneratorSettings : CommonSettings<ClientApiDecoratorsGeneratorSettings>
    {
        protected override string ConfigSectionName { get; } = "clientApiDecoratorsGenerator";

        public IReadOnlyCollection<ClientApiDecoratorsConfiguration> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as ClientApiDecoratorsGenerator;
                return section?.ClientApiDecoratorsConfigurations.Cast<ClientApiDecoratorsConfiguration>().ToList();
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

    [ConfigurationCollection(typeof(ClientApiDecoratorsConfiguration))]
    public class ClientApiDecoratorsConfigurationsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClientApiDecoratorsConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClientApiDecoratorsConfiguration)(element)).SourceInterface;
        }

        public ClientApiDecoratorsConfiguration this[int idx]
        {
            get { return (ClientApiDecoratorsConfiguration)BaseGet(idx); }
        }
    }

    public class ClientApiDecoratorsConfiguration : ConfigurationElement
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
    }
}
