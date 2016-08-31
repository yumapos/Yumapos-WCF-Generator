using System.Collections.Generic;
using WCFGenerator.RepositoriesGeneration.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal class FilterInfo
    {
        public FilterInfo(string key, List<ParameterInfo> parameters)
        {
            Key = key;
            Parameters = parameters;
        }

        public string Key { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
    }
}