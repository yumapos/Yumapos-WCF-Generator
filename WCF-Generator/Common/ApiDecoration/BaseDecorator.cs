using System.Text;
using Microsoft.CodeAnalysis;

namespace WCFGenerator.Common.ApiDecoration
{
    public abstract class BaseDecorator : IDecoratorClass
    {
        public abstract string ClassName { get; }
        public string GetFullText(GenerationConfig config)
        {
            var sb = new StringBuilder();
            GenerateUsings(sb);

            GenerateTemplate(sb, config);

            GenerateMethods(sb, config);

            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        protected abstract void GenerateUsings(StringBuilder sb);

        protected abstract void GenerateTemplate(StringBuilder sb, GenerationConfig config);
        protected abstract void GenerateMethods(StringBuilder sb, GenerationConfig config);

        protected abstract void GenerateMethodBody(StringBuilder sb, IMethodSymbol toDecorate);
    }
}
