﻿using VersionedRepositoryGeneration.Generator.Infrastructure;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class GeneratorHelper
    {
        public static string GetGeneratedDocumentHeader()
        {
            return 
                  "//------------------------------------------------------------------------------\n"
                + "// <auto-generated>\n"
                + "//     This code was generated from a template.\n"
                + "//\n"
                + "//     Manual changes to this file may cause unexpected behavior in your application.\n"
                + "//     Manual changes to this file will be overwritten if the code is regenerated.\n"
                + "// </auto-generated>\n"
                + "//------------------------------------------------------------------------------\n";
        }

        public static string GenerateMethodDeclaration(RepositoryMethod method, string returnType, string parameterType, string parameterName, bool isAsync = false)
        {
            returnType = returnType != null ? '<' + returnType + '>' : null;

            return string.Format("public{0}Task{1}{2}({3}{4})", (isAsync ? " async ":" " ), method.GetName() + (isAsync ? "Async" : ""), returnType ?? " ", parameterType, parameterName);
        }

    }
}
