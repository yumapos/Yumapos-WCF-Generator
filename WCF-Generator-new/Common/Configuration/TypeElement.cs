using System.Configuration;

namespace WCFGenerator.Common.Configuration
{
    public class TypeElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return ((string)(base["type"]));
            }
            set
            {
                base["type"] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(TypeElement))]
    public class TypeElementCollection : ConfigurationElementCollection
    {
        public TypeElement this[int index] => BaseGet(index) as TypeElement;

        protected override ConfigurationElement CreateNewElement()
        {
            return new TypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TypeElement)element).Type;
        }

        public new TypeElementCollection this[string key] => (TypeElementCollection)BaseGet(key);
    }
}