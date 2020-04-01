namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal enum RepositoryMethod
    {
        GetAll,
        Insert,
        InsertMany,
        InsertManySplitByTransactions,
        GetBy,
        UpdateBy,
        UpdateManyBySplitByTransactions,
        RemoveBy,
        InsertOrUpdate
    }
}
