using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace VersionedRepositoryGeneration.Generator.Core
{
    /// <summary>
    ///     Workspace of repository generator - managing project and solution
    /// </summary>
    internal class RepositoryGeneratorWorkSpace
    {
        #region Fields

        private readonly MSBuildWorkspace _workspace;
        private readonly List<SrcFile> _filesToCreation = new List<SrcFile>();

        #endregion

        #region Properties

        public Project Project { get; private set; }
        public Solution Solution { get; private set; }

        #endregion

        #region Constructor

        public RepositoryGeneratorWorkSpace()
        {
            _workspace = MSBuildWorkspace.Create();
        }

        public void OpenSolution(string solutionPath)
        {
            Solution = _workspace.OpenSolutionAsync(solutionPath).Result;
        }

        public void OpenProject(string projectName)
        {
            if (Solution == null) throw new InvalidOperationException("At first open solution");
            Project = Solution.Projects.First(x => x.Name == projectName);
        }

        #endregion

        #region Documents operation

        /// <summary>
        ///     Add new file to list of new file. If file alredy exist, file append removing list.
        /// </summary>
        /// <param name="fileName">File name with extention</param>
        /// <param name="folder">Path to foldel in project</param>
        /// <param name="code">Text of source code</param>
        public void AddFileToCreation(string fileName, string folder, string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentException("code"); 
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("fileName"); 
            if (string.IsNullOrEmpty(folder)) throw new ArgumentException("folder"); 

            _filesToCreation.Add(new SrcFile(fileName, folder, code));
        }

        /// <summary>
        ///     Apply all changes (adding and removing files) for current workspace project
        /// </summary>
        public async Task ApplyChanges()
        {
            var project = Project;
            
            foreach (var doc in _filesToCreation)
            {
                Document document;
                
                var old = project.Documents.FirstOrDefault(x => x.FilePath !=null && x.FilePath.EndsWith(doc.ProjectFolder + "\\" + doc.FileName));
                // check changes
                if (old != null)
                {
                    var st = SourceText.From(doc.SrcText);
                    var newDoc = old.WithText(st);
                    newDoc = Formatting(newDoc);
                    var c = await newDoc.GetTextChangesAsync(old);
                    document = c.Any() ? newDoc : old;
                }
                // create new document
                else
                {
                    document =  project.AddDocument(doc.FileName, doc.SrcText, doc.ProjectFolder.Split('\\'));
                    document = Formatting(document);
                }

                project = document.Project;
            }
            // Apply project changes
            _workspace.TryApplyChanges(project.Solution);
        }

        private Document Formatting(Document doc)
        {
            // general format
            var formattedDoc = Formatter.FormatAsync(doc).Result;
            var text = formattedDoc.GetTextAsync().Result.ToString().Replace("    ", "\t");
            formattedDoc = formattedDoc.WithText(SourceText.From(text));
            return formattedDoc;
        }

        #endregion

        #region Private

        private struct SrcFile
        {
            public readonly string FileName;
            public readonly string ProjectFolder;
            public readonly string SrcText;

            /// <summary>
            ///     New source code file
            /// </summary>
            /// <param name="fileName">File name with extension</param>
            /// <param name="projectFolder">Folder in project where file can be saved (for example: "Models\Generated" )</param>
            /// <param name="srcText">Text of code</param>
            public SrcFile(string fileName, string projectFolder, string srcText)
            {
                FileName = fileName;
                ProjectFolder = projectFolder;
                SrcText = srcText;
            }
        }

        #endregion
    }
}