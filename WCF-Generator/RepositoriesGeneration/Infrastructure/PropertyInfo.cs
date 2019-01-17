namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class PropertyInfo
    {
        public PropertyInfo(string name, bool isParameter = true, bool isNullable = false, bool isDataType = false)
        {
            Name = name;
            IsParameter = isParameter;
            IsNullable = isNullable;
            IsDataType = isDataType;
        }

        public string Name { get; }
        public bool IsNullable { get; }
        public bool IsParameter { get; }
        public bool IsDataType { get; }
    }
}
