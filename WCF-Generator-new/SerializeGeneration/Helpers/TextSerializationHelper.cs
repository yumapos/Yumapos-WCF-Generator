using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.SerializeGeneration.Models;

namespace WCFGenerator.SerializeGeneration.Helpers
{
    public class TextSerializationHelper
    {
        


        public static IEnumerable<string> GetUsingNamespaces(ClassDeclarationSyntax classes)
        {
            var syntaxTree = classes.SyntaxTree;
            var root = (CompilationUnitSyntax)syntaxTree.GetRoot();


            return root.Usings.Select(x => x.Name.ToString());
        }

        public static bool IsCollectionEquals(List<GenerationProperty> firstCollection, List<GenerationProperty> secondCollection)
        {
            if (firstCollection.Count != secondCollection.Count)
                return false;
            foreach (var generationProperty in firstCollection)
            {
                var prop = secondCollection.FirstOrDefault(x => x.Name == generationProperty.Name);
                if (prop == null || prop.Type.Trim() != generationProperty.Type.Trim())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
