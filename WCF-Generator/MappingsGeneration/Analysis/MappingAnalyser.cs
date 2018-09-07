using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WCFGenerator.Common;
using WCFGenerator.MappingsGeneration.Configuration;

namespace WCFGenerator.MappingsGenerator.Analysis
{
    public class MappingAnalyser
    {
        private readonly MappingConfiguration _configuration;
        private readonly GeneratorWorkspace _generatorWorkspace;

        public MappingAnalyser(MappingConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
        }

 /*       public string GetMapName(CSharpSyntaxNode element)
        {
            string value;
            //var value = element.Name;
            const string nameProperty = "Name";

            List<AttributeSyntax> attributes = new List<AttributeSyntax>();

            if (element is ClassDeclarationSyntax codeClass)
            {
                foreach (var codeClassAttributeList in codeClass.AttributeLists)
                {
                    attributes.AddRange(codeClassAttributeList.Attributes);
                }

                value = codeClass.Identifier.Text;
            }

            if (element is PropertyDeclarationSyntax codeProperty)
            {
                foreach (var codeClassAttributeList in codeProperty.AttributeLists)
                {
                    attributes.AddRange(codeClassAttributeList.Attributes);
                }

                value = codeProperty.Identifier.Text;
            }

            foreach (var ca in attributes)
            {
                if (ca.Name.ToString().Contains(_configuration.MapAttribute) && ca.ArgumentList.Arguments.FirstOrDefault(a => a.Value.Contains(nameProperty)) )
                {
                    value = ca.Value.Remove(0, ca.Value.IndexOf(nameProperty));
                    value = value.Replace(" ", "");

                    if (value.Contains(","))
                    {
                        value = value.Remove(value.IndexOf(","));
                    }

                    value = value.Remove(0, nameProperty.Length + 1);
                    value = value.Replace("\"", "").ToLower();

                    if (DoSuffix != null && codeClass != null)
                    {
                        if (value.EndsWith(DoSuffix.ToLower()))
                        {
                            value = value.Replace(DoSuffix.ToLower(), "");
                        }
                    }

                    if (DtoSuffix != null && codeClass != null)
                    {
                        if (value.EndsWith(DtoSuffix.ToLower()))
                        {
                            value = value.Replace(DtoSuffix.ToLower(), "");
                        }
                    }
                }
            }

            value = value.ToLower();

            if (DoSuffix != null && codeClass != null)
            {
                if (value.EndsWith(DoSuffix.ToLower()))
                {
                    value = value.Replace(DoSuffix.ToLower(), "");
                }
            }

            if (DtoSuffix != null && codeClass != null)
            {
                if (value.EndsWith(DtoSuffix.ToLower()))
                {
                    value = value.Replace(DtoSuffix.ToLower(), "");
                }
            }

            return value;
        }*/
    }
}
