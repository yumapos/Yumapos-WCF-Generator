using System.Collections.Generic;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class FilterInfo
    {
        public FilterInfo(string key, List<ParameterInfo> parameters, FilterType filterType)
        {
            Key = key;
            Parameters = parameters;
            FilterType = filterType;
        }

        /// <summary>
        ///     Name of primary or filter key (for key based method only)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Type of filter for key
        /// </summary>
        public FilterType FilterType { get; set; }

        public List<ParameterInfo> Parameters { get; set; }
    }
}