namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal enum RepositoryMethod
    {
        GetAll,
        Insert,
        GetBy,
        UpdateBy,
        RemoveBy
    }

    internal enum FilterType
    {
        PrimaryKey,
        FilterKey,
        VersionKey
    }
}
