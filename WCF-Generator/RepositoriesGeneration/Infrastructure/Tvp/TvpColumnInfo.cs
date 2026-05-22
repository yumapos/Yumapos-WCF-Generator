namespace WCFGenerator.RepositoriesGeneration.Infrastructure.Tvp;

public class TvpColumnInfo
{
    public string DoPropertyName { get; set; }
    public string DoFullQualifiedClrType { get; set; } // full original type
    public string DoClrUnderlying { get; set; }
    public string DataTableColumnName { get; set; } // name of column in datatable
    public string DataTableSqlType { get; set; } // sql type in datatable
    public bool IsNullable { get; set; }
    public bool IsEnum { get; set; }
    public bool IsEnumStoredAsStringInDataBase { get; set; }
}