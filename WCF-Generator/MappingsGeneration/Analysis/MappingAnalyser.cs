using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
        private Solution _solution = null;

        IDictionary<string, CSharpCompilation> _doProjectsCompilations = new Dictionary<string, CSharpCompilation>();

        public MappingAnalyser(MappingConfiguration configuration, GeneratorWorkspace generatorWorkspace)
        {
            _configuration = configuration;
            _generatorWorkspace = generatorWorkspace;
            _solution = _generatorWorkspace.Solution;
        }

        public async Task Run()
        {
            StringBuilder sb = new StringBuilder();

            if (_configuration.DoProjects == null || _configuration.DoProjects.Count == 0)
            {
                throw new Exception("List of DoProjects doesn't exist");
            }

            if (_configuration.DtoProjects == null || _configuration.DoProjects.Count == 0)
            {
                throw new Exception("List of DtoProjects doesn't exist");
            }

            if (string.IsNullOrEmpty(_configuration.MapExtensionClassName))
            {
                throw new Exception("Name of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(_configuration.MapExtensionNameSpace))
            {
                throw new Exception("Namespace of Generated Class wasn't set");
            }

            if (string.IsNullOrEmpty(_configuration.MapAttribute))
            {
                throw new Exception("Attribute for Mapping doesn't exist");
            }

            var classesWithMapAttribute = new List<ClassDeclarationSyntax>();
            var classesWithMapAttributeDto = new List<ClassDeclarationSyntax>();

            foreach (MappingSourceProject project in _configuration.DoProjects)
            {
                classesWithMapAttribute.AddRange(
                    await GetAllClasses(project.ProjectName, _configuration.DOSkipAttribute, _configuration.MapAttribute));
            }

            foreach (MappingSourceProject project in _configuration.DtoProjects)
            {
                classesWithMapAttributeDto.AddRange(
                    await GetAllClasses(project.ProjectName, _configuration.DOSkipAttribute, _configuration.MapAttribute));
            }
        }

        public async Task<IEnumerable<ClassDeclarationSyntax>> GetAllClasses(string projectName, bool isSkipAttribute, string attribute)
        {
            var project = _solution.Projects.First(x => x.Name == projectName);
            var compilation = await project.GetCompilationAsync();
            var classVisitor = new ClassVirtualizationVisitor();
            var classes = new List<ClassDeclarationSyntax>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                classVisitor.Visit(syntaxTree.GetRoot());
            }

            if (!isSkipAttribute)
            {
                classes = classVisitor.Classes.Where(x => x.AttributeLists
                    .Any(att => att.Attributes
                        .Any(att2 => att2.Name.ToString() == attribute))).ToList();
            }
            else
            {
                classes = classVisitor.Classes;
            }

            return classes;
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
