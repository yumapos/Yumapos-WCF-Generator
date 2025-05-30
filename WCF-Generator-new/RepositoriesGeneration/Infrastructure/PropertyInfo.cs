using System;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class PropertyInfo
    {
        private PropertyIgnoreOptions _ignoreOptions;

        public PropertyInfo(string name,
            bool isParameter = true,
            bool isNullable = false,
            bool isBool = false,
            bool isEnum = false,
            bool cultureDependent = false,
            PropertyIgnoreOptions ignoreOptions = PropertyIgnoreOptions.None)
        {
            Name = name;
            IsParameter = isParameter;
            IsNullable = isNullable;
            IsBool = isBool;
            IsEnum = isEnum;
            CultureDependent = cultureDependent;
            _ignoreOptions = ignoreOptions;
        }

        public bool IsEnum { get; }
        public string Name { get; }
        public bool IsNullable { get; }
        public bool IsParameter { get; }
        public bool IsBool { get; }
        public bool CultureDependent { get; set; }
        public bool IgnoreAlways => _ignoreOptions == PropertyIgnoreOptions.Always;
        public bool IgnoreOnUpdate => IgnoreAlways || _ignoreOptions.HasFlag(PropertyIgnoreOptions.OnUpdate);
    }

    [Flags]
    internal enum PropertyIgnoreOptions
    {
        None = 0x0,
        Always = 0x1,
        OnUpdate = 0x2
    }
}
