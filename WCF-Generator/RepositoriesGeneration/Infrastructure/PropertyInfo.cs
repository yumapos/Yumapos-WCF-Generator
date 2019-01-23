namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class PropertyInfo
    {
        public PropertyInfo(string name, bool isParameter = true, bool isNullable = false, bool isBool = false, bool isEnum = false)
        {
            Name = name;
            IsParameter = isParameter;
            IsNullable = isNullable;
            IsBool = isBool;
            IsEnum = isEnum;
        }

        public bool IsEnum { get; }
        public string Name { get; }
        public bool IsNullable { get; }
        public bool IsParameter { get; }
        public bool IsBool { get; }
    }
}
