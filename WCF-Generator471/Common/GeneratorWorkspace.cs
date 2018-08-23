using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using WCFGenerator.Common;

namespace WCF_Generator471.Common
{
    /// <summary>
    ///     Workspace for any generators of code
    /// </summary>
    public class GeneratorWorkspace
    {
        /* How it work:
         * 1. Create new GeneratorWorkspace by absolute solution path 
         * 2. Set target project for saving code
         * 3. Update files in project (create new)
         * 4. Apply target project changes
         * 5. Close solution if all project updated
         */

        public GeneratorWorkspace(string absoluteSlnPath)
        {
            MsBuildWorkspace = MSBuildWorkspace.Create();
            Solution = MsBuildWorkspace.OpenSolutionAsync(absoluteSlnPath).Result;
            var workspaceDiagnostics = MsBuildWorkspace.Diagnostics;
            foreach (var workspaceDiagnostic in workspaceDiagnostics)
            {
                if (!workspaceDiagnostic.Message.Contains(".Test"))
                {
                    throw new InvalidOperationException("Build error " + workspaceDiagnostic);
                }
            }
        }

        public Solution Solution { get; set; }
        public MSBuildWorkspace MsBuildWorkspace { get; private set; }

        public bool ApplyChanges()
        {
            var ret = MsBuildWorkspace.TryApplyChanges(Solution);
            Solution = MsBuildWorkspace.CurrentSolution;
            return ret;
        }

        public void CloseSolution()
        {
            ApplyChanges();
            MsBuildWorkspace.CloseSolution();
        }

        #region Fields

        private readonly List<SrcFile> _filesToCreation = new List<SrcFile>();

        #endregion

        #region Properties

        public Project Project { get; private set; }

        #endregion

        /// <summary>
        ///     Set target project for saving generated code.
        /// </summary>
        /// <param name="projectName"></param>
        public void SetTargetProject(string projectName)
        {
            if (Solution == null) throw new InvalidOperationException("At first open solution");
            var inttt = Solution.Projects.Count();
            Project = Solution.Projects.First(x => x.Name == projectName);
            _filesToCreation.Clear();
        }

        #region Documents operation

        /// <summary>
        ///     Add new file to list of new file. If file alredy exist, file append removing list.
        /// </summary>
        /// <param name="fileName">File name with extention</param>
        /// <param name="folder">Path to foldel in project</param>
        /// <param name="code">Text of source code</param>
        public void UpdateFileInTargetProject(string fileName, string folder, string code)
        {
            var file = new SrcFile(fileName, folder, code);
            UpdateFileInTargetProject(file);
        }
        public void UpdateFileInTargetProject(SrcFile file)
        {
            ValidateFile(file);
            _filesToCreation.Add(file);
        }

        public void UpdateFileInTargetProject(List<SrcFile> files)
        {
            foreach (var file in files)
            {
                UpdateFileInTargetProject(file);
            }
        }

        private void ValidateFile(SrcFile file)
        {
            if (string.IsNullOrEmpty(file.SrcText)) throw new ArgumentException("SrcText");
            if (string.IsNullOrEmpty(file.FileName)) throw new ArgumentException("FileName");
            if (file.ProjectFolder == null) throw new ArgumentException("ProjectFolder");
            if (_filesToCreation.Any(f => f.FileName == file.FileName && f.ProjectFolder == file.ProjectFolder)) throw new ArgumentException(file.FileName + " - file already exists.");
        }

        /// <summary>
        ///     Apply all changes (adding and removing files) for current workspace project
        /// </summary>
        public async Task ApplyTargetProjectChanges(bool standartFormatting = false)
        {
            var project = Project;

            foreach (var doc in _filesToCreation)
            {
                Document document;

                var old = project.Documents.FirstOrDefault(x => x.FilePath != null && x.FilePath.EndsWith(doc.ProjectFolder + "\\" + doc.FileName));

                var code = CodeHelper.GeneratedDocumentHeader + "\r\n" + doc.SrcText;

                // check changes
                if (old != null)
                {
                    var st = SourceText.From(code);
                    var newDoc = old.WithText(st);
                    if (standartFormatting)
                    {
                        newDoc = Formatting(newDoc);
                    }
                    var c = await newDoc.GetTextChangesAsync(old);
                    document = c.Any() ? newDoc : old;
                    project = document.Project;
                }
                // create new document
                else
                {
                    document = project.AddDocument(doc.FileName, code, doc.ProjectFolder.Split('\\'));
                    if (standartFormatting)
                    {
                        document = Formatting(document);
                    }

                    // this is workaround to Roslyn adding strings <Compile Include="Repositories\Generated\CashDrawerCheckRepository.g.cs" />
                    // to a project file if add file directly to Roslyn
                    var documentText = document.GetTextAsync().Result.ToString();
                    var lastOccur = project.FilePath.Split(new []{'\\'}).Last().Length;
                    var path = project.FilePath.Substring(0, project.FilePath.Length - lastOccur) + doc.ProjectFolder + @"\" + doc.FileName;
                    System.IO.File.WriteAllText(path, documentText, Encoding.UTF8);
                }
            }
            // Apply project changes
            Solution = project.Solution;
            ApplyChanges();
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
    }
}
