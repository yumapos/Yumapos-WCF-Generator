namespace WCFGenerator.Common.Infrastructure
{
    internal interface ICodeClassGenerator
    {
        string GetUsings();
        string GetNamespaceDeclaration();
        string GetClassDeclaration();
        string GetFields();
        string GetProperties();
        string GetConstructors();
        string GetMethods();
        string GetFullCode();
    }
}