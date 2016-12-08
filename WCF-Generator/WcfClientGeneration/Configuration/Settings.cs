﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WCFGenerator.WcfClientGeneration.Configuration
{
    public static class WcfServiceClientGeneratorSettings
    {
        public static IReadOnlyCollection<WcfService> GetConfigs()
        {
            try
            {
                var section = ConfigurationManager.GetSection("wcfClientGenerator") as WcfClientGenerator;
                if (section == null) throw new ConfigurationErrorsException("wcfClientGenerator section not found at " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                return section.RepositoryProjects.Cast<WcfService>().ToList();
            }
            catch (ConfigurationErrorsException e)
            {
                throw new InvalidOperationException("Error occurred on trying get Wcf Client Generator settings.", e);
            }
        }

        public static string GetSolutionPath()
        {
            var ret = ConfigurationManager.AppSettings["SolutionPath"];
            return ret;
        }
    }

    /// <summary>
    ///     Configuration section "WcfClientGenerator"
    /// </summary>
    public class WcfClientGenerator : ConfigurationSection
    {
        /// <summary>
        ///     All configured wcf services for generate client
        /// </summary>
        [ConfigurationProperty("wcfServices")]
        public WcfServices RepositoryProjects
        {
            get { return ((WcfServices)(base["wcfServices"])); }
        }
    }

    /// <summary>
    ///     RepositoryProjects collection element
    /// </summary>
    [ConfigurationCollection(typeof(WcfService))]
    public class WcfServices : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WcfService();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WcfService)(element)).ClientInterfaceName;
        }

        public WcfService this[int idx]
        {
            get { return (WcfService)BaseGet(idx); }
        }
    }

    /// <summary>
    ///     Service for generate wcf client
    /// </summary>
    public class WcfService : ConfigurationElement
    {
        /// <summary>
        ///     Name of client interface
        /// </summary>
        [ConfigurationProperty("ClientInterfaceName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ClientInterfaceName
        {
            get { return ((string)(base["ClientInterfaceName"])); }
        }

        /// <summary>
        ///     Name of file client interface
        /// </summary>
        [ConfigurationProperty("ServiceInterfaceFileName", DefaultValue = "", IsRequired = true)]
        public string ClientInterfaceFileName
        {
            get { return ((string)(base["ServiceInterfaceFileName"])); }
        }

        /// <summary>
        ///     Name of project for save client
        /// </summary>
        [ConfigurationProperty("TargetProjectName", DefaultValue = "", IsRequired = true)]
        public string TargetProjectName
        {
            get { return ((string)(base["TargetProjectName"])); }
        }

        /// <summary>
        ///     Namespce of faults
        /// </summary>
        [ConfigurationProperty("FaultNamespace", DefaultValue = "", IsRequired = true)]
        public string FaultNamespace
        {
            get { return ((string)(base["FaultNamespace"])); }
        } 
        
        /// <summary>
        ///     Name of project for api interfaces
        /// </summary>
        [ConfigurationProperty("ApiInterfaceProjectName", DefaultValue = "", IsRequired = true)]
        public string ApiInterfaceProjectName
        {
            get { return ((string)(base["ApiInterfaceProjectName"])); }
        }

        /// <summary>
        ///     Name of folder in project for api interfaces
        /// </summary>
        [ConfigurationProperty("ApiInterfaceProjectFolder", DefaultValue = "", IsRequired = true)]
        public string ApiInterfaceProjectFolder
        {
            get { return ((string)(base["ApiInterfaceProjectFolder"])); }
        }
    }
}