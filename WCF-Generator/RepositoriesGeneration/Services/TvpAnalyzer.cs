using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.RepositoriesGeneration.Analysis;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Infrastructure.Tvp;

namespace WCFGenerator.RepositoriesGeneration.Services;

internal class TvpAnalyzer
{
    private readonly SolutionSyntaxWalker _syntaxWalker;

    public TvpAnalyzer(SolutionSyntaxWalker syntaxWalker)
    {
        _syntaxWalker = syntaxWalker;
    }

    public void PopulateTvpMetadata(RepositoryInfo repositoryInfo, ClassDeclarationSyntax modelClass)
    {
        if (repositoryInfo == null || modelClass == null)
            return;

        var tvp = repositoryInfo.TvpMetadata ?? new TvpMetadata();
        repositoryInfo.TvpMetadata = tvp;

        var splitted = repositoryInfo.TableName.Split(".").Select(x => x.Trim('[').Trim(']')).ToList();
        if (splitted.Count == 1)
        {
            tvp.TableName = splitted.First();
        }
        else
        {
            tvp.TableName = splitted.Last();
            tvp.Schema = splitted.First();
        }

        if (repositoryInfo.IsVersioning)
        {
            var versionSplitted = repositoryInfo.VersionTableName.Split(".").Select(x => x.Trim('[').Trim(']')).ToList();
            if (versionSplitted.Count == 1)
            {
                tvp.VersionTableName = versionSplitted.First();
            }
            else
            {
                tvp.VersionTableName = versionSplitted.Last();
                tvp.VersionSchema = versionSplitted.First();
            }
        }
        
        tvp.DataTableName = $"{tvp.Schema}.UT_{tvp.TableName}";

        var properties = modelClass.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(p => p.GetterExist() && p.SetterExist())
            .ToList();

        foreach (var prop in properties)
        {
            var ignoreOptions = GetIgnoreOptions(modelClass, prop);
            if (ignoreOptions == PropertyIgnoreOptions.Always) continue;

            var (fullClrType, isNullable) = _syntaxWalker.GetFullPropertyTypeNameAndNullable(prop);
            if (fullClrType=="System.String")
            {
                isNullable = true;
            }
            var isEnum = _syntaxWalker.PropertyIsEnum(prop);
            var isEnumString = false;
            var clrUnderlyingType = fullClrType;
            string sqlType;

            if (isEnum)
            {
                isEnumString = prop.AttributeExist("EnumStringValue");
                if (isEnumString)
                {
                    clrUnderlyingType = "System.String";
                    sqlType = SystemToSqlTypeMapper.GetSqlType(clrUnderlyingType);
                }
                else
                {
                    var underlyingFullType = _syntaxWalker.GetEnumUnderlyingTypeFullName(prop);
                    clrUnderlyingType = underlyingFullType;
                    sqlType = SystemToSqlTypeMapper.GetSqlType(underlyingFullType);
                }
            }
            else
            {
                sqlType = SystemToSqlTypeMapper.GetSqlType(fullClrType);
            }

            if (string.IsNullOrEmpty(sqlType))
            {
                throw new ArgumentException($"Unknown system type: {fullClrType}");
            }

            var columnInfo = new TvpColumnInfo
            {
                DoPropertyName = prop.Identifier.Text,
                DoFullQualifiedClrType = fullClrType,
                DoClrUnderlying = clrUnderlyingType,
                DataTableColumnName = prop.Identifier.Text,
                DataTableSqlType = sqlType,
                IsNullable = isNullable,
                IsEnum = isEnum,
                IsEnumStoredAsStringInDataBase = isEnumString,
            };
            tvp.AddColumn(columnInfo);
        }

        if (repositoryInfo.IsTenantRelated &&
            !tvp.OrderedColumns.Any(x => x.DataTableColumnName.Equals("tenantId", StringComparison.OrdinalIgnoreCase)))
        {
            tvp.AddColumn(new TvpColumnInfo()
            {
                DoPropertyName = "TenantId",
                DoFullQualifiedClrType = "System.Guid",
                DataTableColumnName = "TenantId",
                IsNullable = false,
                DataTableSqlType = "uniqueidentifier",
                DoClrUnderlying = "Guid",
                IsEnum = false,
                IsEnumStoredAsStringInDataBase = false,
            });
        }
    }

    private PropertyIgnoreOptions GetIgnoreOptions(ClassDeclarationSyntax modelClass, PropertyDeclarationSyntax property)
    {
        if (!property.AttributeExist(RepositoryDataModelHelper.DbIgnoreAttributeName))
            return PropertyIgnoreOptions.None;

        var propertyName = property.Identifier.Text;
        var value = _syntaxWalker.GetAttributeArgumentValue(modelClass,
            propertyName,
            RepositoryDataModelHelper.DbIgnoreAttributeName,
            "IgnoreOnUpdate");

        if (value == null || !bool.TryParse(value, out var ignoreOnUpdate))
            return PropertyIgnoreOptions.Always;

        return ignoreOnUpdate ? PropertyIgnoreOptions.OnUpdate : PropertyIgnoreOptions.Always;
    }
}