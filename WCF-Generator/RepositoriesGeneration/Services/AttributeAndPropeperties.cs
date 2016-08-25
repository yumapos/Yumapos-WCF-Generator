using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Models.Attributes;

namespace VersionedRepositoryGeneration.Generator.Services
{
    internal class AttributeAndPropeperties
    {
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string GetParameterByKeyName(string key)
        {
            return Parameters.FirstOrDefault(x => x.Key.ToString().Trim() == key).Value;
        }
    }
}