using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFGenerator.Common.Configuration
{
    [ConfigurationCollection(typeof(PrefixString))]
    public class PrefixStrings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PrefixString();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PrefixString)(element)).Text;
        }

        public PrefixString this[int idx]
        {
            get { return (PrefixString)BaseGet(idx); }
        }
    }

    public class PrefixString : ConfigurationElement
    {
        [ConfigurationProperty("text", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Text
        {
            get { return ((string)(base["text"])); }
        }
    }
}
