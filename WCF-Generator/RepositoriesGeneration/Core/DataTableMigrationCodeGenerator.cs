using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.Common.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Configuration;
using WCFGenerator.RepositoriesGeneration.Enums;
using WCFGenerator.RepositoriesGeneration.Helpers;
using WCFGenerator.RepositoriesGeneration.Infrastructure;
using WCFGenerator.RepositoriesGeneration.Infrastructure.Tvp;

namespace WCFGenerator.RepositoriesGeneration.Core;


internal class DataTableMigrationCodeGenerator
{
    private readonly GeneratorWorkspace _generatorWorkspace;
    private readonly IEnumerable<RepositoryCodeGeneratorAbstract> _repositoryCodeGeneratorAbstracts;
    private readonly RepositoryProject _projectConfig;
    private readonly long _migrationId = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

    public DataTableMigrationCodeGenerator(
        GeneratorWorkspace generatorWorkspace,
        RepositoryProject projectConfig,
        IEnumerable<RepositoryCodeGeneratorAbstract> repositoryCodeGeneratorAbstracts)
    {
        _generatorWorkspace = generatorWorkspace;
        _projectConfig = projectConfig;
        _repositoryCodeGeneratorAbstracts = repositoryCodeGeneratorAbstracts;
    }
    
    public async Task<string> GetFullCode()
    {
        if (_projectConfig.InsertManyMethod != InsertManyMethod.ViaTvp)
        {
            return "";
        }
        if (_repositoryCodeGeneratorAbstracts == null || !_repositoryCodeGeneratorAbstracts.Any())
        {
            return "";
        }

        if (_projectConfig.ForceRecreateMigrations)
        {
            return GetForce();
        }

        return await GetIncremental();
    }

    private string GetForce()
    {
        var reposInfos = _repositoryCodeGeneratorAbstracts
            .Where(x=> InsertManyRealiazationExists(x.GetFullCode()))
            .Select(x => x.RepositoryInfo);
        reposInfos = PopulateJoinedTablesInfo(reposInfos);
        var sb = new StringBuilder();
        sb.AppendLine(GetUsings());
        sb.AppendLine();
        sb.AppendLine(GetNamespaceDeclaration());
        sb.AppendLine(GetMigrationDeclaration(reposInfos));
        return sb.ToString();
    }

    private async Task<string> GetIncremental()
    {
        var infos = new List<RepositoryInfo>();
        var repositoryProject = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == _projectConfig.TargetProjectName);
        var migrationProject = _generatorWorkspace.Solution.Projects.FirstOrDefault(x => x.Name == _projectConfig.DataTableMigrationProjectName);
        foreach (var repositoryCodeGeneratorAbstract in _repositoryCodeGeneratorAbstracts)
        {
            var newCode = repositoryCodeGeneratorAbstract.GetFullCode();
            var needToCreateDataTableMigration = await NeedToCreateDataTableMigration(repositoryProject, _projectConfig,
                repositoryCodeGeneratorAbstract, newCode);
            if (needToCreateDataTableMigration)
            {
                infos.Add(repositoryCodeGeneratorAbstract.RepositoryInfo);
            }
        }

        infos = PopulateJoinedTablesInfo(infos);

        if (infos.Count == 0)
        {
            return "";
        }
        var migrationClassDeclaration = GetMigrationDeclaration(infos);
        var fileName = GetFileName();
        var oldDocument = migrationProject.Documents.FirstOrDefault(x => x.FilePath.NormalizeDirectorySlashes().EndsWith(fileName));
        if (oldDocument != null)
        {
            var syntaxTree = await oldDocument.GetSyntaxTreeAsync();
            var root = await syntaxTree.GetRootAsync() as CompilationUnitSyntax;
            if (root == null) return null;
            
            root = root.WithLeadingTrivia(SyntaxFactory.TriviaList());
            
            var newClass = SyntaxFactory.ParseMemberDeclaration(migrationClassDeclaration);
            if (newClass == null) return null;
            newClass = newClass.WithLeadingTrivia(
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.CarriageReturnLineFeed
            );
            var newRoot = root.AddMembers(newClass);
            return newRoot.ToFullString();
        }
        var sb = new StringBuilder();
        sb.AppendLine(GetUsings());
        sb.AppendLine();
        sb.AppendLine(GetNamespaceDeclaration());
        sb.AppendLine(migrationClassDeclaration);
        return sb.ToString();
    }
    
    public string GetFileName()
    {
        return $"CreateDatatypes_{_projectConfig.Name}.g.cs";
    }

    
    
    private string GetMigrationDeclaration(IEnumerable<RepositoryInfo> repositoryInfos)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    {GetClassDeclaration()}");
        sb.AppendLine("{");
        sb.Append(GetMethods(repositoryInfos));
        sb.AppendLine("}");
        return sb.ToString();
    }
    
    private string GetUsings()
    {
        return """
               using System;
               using FluentMigrator;
               """;
    }

    private string GetNamespaceDeclaration()
    {
        return $"namespace {_projectConfig.DataTableMigrationNamespace};";
    }

    private string GetClassDeclaration()
    {
        var className = GetSafeClassName(_migrationId);
        return $"""
                [Migration({_migrationId})]
                public class {className} : FluentMigrator.Migration
                """;
    }

    private string GetMethods(IEnumerable<RepositoryInfo> repositoryInfos)
    {
        var upCode = GenerateUpCode(repositoryInfos);
        var downCode = @"throw new NotImplementedException();";

        return $@"
        public override void Up()
        {{
{upCode}
        }}

        public override void Down()
        {{
            {downCode}
        }}";
    }
    
    private string GetSafeClassName(long migrationId)
    {
        return $"M{migrationId}_CreateTVP_{_projectConfig.Name}".Replace(".", "_");
    }

    private string GenerateUpCode(IEnumerable<RepositoryInfo> repositoryInfos)
    {
        var sb = new StringBuilder();
        var orderedInfos = repositoryInfos.OrderBy(x => x.TvpMetadata.DataTableName).ToList();
        foreach (var repositoryInfo in orderedInfos)
        {
            var sql = GenerateCreateTypeSql(repositoryInfo.TvpMetadata);
            var escapedSql = sql.Replace("\"", "\"\"");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"-- For table {repositoryInfo.TableName}, DO: {repositoryInfo.ClassName}");
            sb.AppendLine();
            sb.Append(escapedSql);
            sb.AppendLine();
        }
        var fullSql =  sb.ToString();
        return $"Execute.Sql(@\"{fullSql}\");";
    }

    private string GenerateCreateTypeSql(TvpMetadata tvp)
    {
        var columns = tvp.OrderedColumns;
        var colDefs = columns.Select(c => $"    [{c.DataTableColumnName}] {c.DataTableSqlType} {(c.IsNullable ? "NULL" : "NOT NULL")}");
        return $"""
                IF TYPE_ID(N'{tvp.DataTableName}') IS NOT NULL
                    DROP TYPE {tvp.DataTableName};
                CREATE TYPE {tvp.DataTableName} AS TABLE (
                {string.Join(",\n", colDefs)}
                );
                """;
    }

    private bool InsertManyRealiazationExists(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var method = FindMethod(syntaxTree.GetRoot(), "InsertManyViaTvp");
        return method != null;
    }

    private List<RepositoryInfo> PopulateJoinedTablesInfo(IEnumerable<RepositoryInfo> repositoryInfos)
    {
        var joinedTables = repositoryInfos.Select(x => x.JoinRepositoryInfo)
            .Where(x => x != null);
        return repositoryInfos.Concat(joinedTables).Distinct().ToList();
    }
    
    private async Task<bool> NeedToCreateDataTableMigration(Project repositoryProject, RepositoryProject project, ICodeClassGeneratorRepository repository, string newCode)
    {
        var newFilePath = Path.Combine(project.RepositoryTargetFolder, repository.FileName).NormalizeDirectorySlashes();
        var oldDocument = repositoryProject.Documents
            .FirstOrDefault(x => x.FilePath != null &&
                                 x.FilePath.NormalizeDirectorySlashes().EndsWith(newFilePath));
        if (oldDocument == null)
        {
            return true;
        }
        var oldSyntaxTree = await oldDocument.GetSyntaxTreeAsync();
        var newSyntaxTree = CSharpSyntaxTree.ParseText(newCode);
            
        var oldMethod = FindMethod(await oldSyntaxTree.GetRootAsync(), "InsertManyViaTvp");
        var newMethod = FindMethod(await newSyntaxTree.GetRootAsync(), "InsertManyViaTvp");
    
        return !SyntaxFactory.AreEquivalent(oldMethod, newMethod);
    } 
    private MethodDeclarationSyntax FindMethod(SyntaxNode root, string methodName)
    {
        return root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.Text.StartsWith(methodName));
    }
    
}
