using System.Configuration;

namespace WCFGenerator.Common.ApiDecoration
{
    public class ApiDecoratorsConfiguration : ConfigurationElement
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
            get { return ((string)(base["partialClass"])); }
        }
    }
}
