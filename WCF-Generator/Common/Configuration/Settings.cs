using System.Configuration;

namespace WCFGenerator.Common
{
    public abstract class CommonSettings<T> where T : new()
    {
        protected abstract string ConfigSectionName { get; }

        public static T Current { get; } = new T();

        public bool Enabled
        {
            get
            {
                var section = ConfigurationManager.GetSection(ConfigSectionName) as BasicConfigurationSection;
                return section?.Enabled ?? false;
            }
        }

        public string GetSolutionPath()
        {
            var ret = ConfigurationManager.AppSettings["SolutionPath"];
            return ret;
        }
    }

    /// <summary>
    ///     Section as basic configuration section
    /// </summary>
    public class BasicConfigurationSection : ConfigurationSection
    {
        /// <summary>
        ///     Enabled
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled => ((bool)(base["enabled"]));
    }
}
