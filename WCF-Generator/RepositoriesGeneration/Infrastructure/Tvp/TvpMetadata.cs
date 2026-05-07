namespace WCFGenerator.RepositoriesGeneration.Infrastructure.Tvp;

public class TvpMetadata
{
    public string AnalysisError { get; set; }
    public string DataTableName { get; set; }
    public string TableName { get; set; }
    public string VersionTableName { get; set; }
    public string Schema { get; set; } = "dbo";
    public string VersionSchema { get; set; } = "dbo";
    
    private readonly SortedSet<TvpColumnInfo> _orderedColumns = new(TvpColumnComparer.Instance);
    public IReadOnlyCollection<TvpColumnInfo> OrderedColumns => _orderedColumns;
    public void AddColumn(TvpColumnInfo column)
    {
        _orderedColumns.Add(column);
    }
    
    private sealed class TvpColumnComparer : IComparer<TvpColumnInfo>
    {
        public static readonly TvpColumnComparer Instance = new();
        private TvpColumnComparer(){}
    
        public int Compare(TvpColumnInfo x, TvpColumnInfo y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return string.CompareOrdinal(x.DoPropertyName, y.DoPropertyName);
        }
    }
}