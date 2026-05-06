using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace WCFGenerator.Common
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

        private static int CountProjectsInSolution(string solutionPath)
        {
            var solutionFolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
            int count = 0;
    
            foreach (string line in File.ReadLines(solutionPath))
            {
                if (!line.TrimStart().StartsWith("Project(\"")) continue;
        
                // Извлекаем GUID типа проекта
                var match = System.Text.RegularExpressions.Regex.Match(line, @"Project\(""\{([^}]+)\}""\)");
                if (match.Success)
                {
                    string guid = "{" + match.Groups[1].Value + "}";
                    if (guid != solutionFolderGuid) // исключаем папки
                        count++;
                }
            }
            return count;
        }
        public GeneratorWorkspace(string absoluteSlnPath)
        {
            MsBuildWorkspace = MSBuildWorkspace.Create();
            
            int totalProjects;
            var progress = new Progress<ProjectLoadProgress>();
            try
            {
                totalProjects = CountProjectsInSolution(absoluteSlnPath);
                Console.WriteLine("Total projects: {0}", totalProjects);
                var loadedProjects = 0;
                progress.ProgressChanged += (sender, loadProgress) =>
                {
                    if (loadProgress.Operation == ProjectLoadOperation.Build && !string.IsNullOrEmpty(loadProgress.FilePath))
                    {
                        loadedProjects += 1;
                    }
                    var percent = (double)loadedProjects / totalProjects * 100;
                    var barLength = 50;
                    var filled = (int)(percent / 100.0 * barLength);
                    var bar = new string('█', filled) + new string('░', barLength - filled);
            
                    var message = $"[{bar}] {percent,5:F1}%";
            
                    Console.Write("\r" + message.PadRight(Console.WindowWidth - 1));
                };
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to count projects in solution {absoluteSlnPath}");
            }
            Solution = MsBuildWorkspace.OpenSolutionAsync(absoluteSlnPath, progress).Result;
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
            var workspaceDiagnostics = MsBuildWorkspace.Diagnostics;
            foreach (var workspaceDiagnostic in workspaceDiagnostics)
            {
                if (!workspaceDiagnostic.Message.Contains(".Test"))
                {
                    //throw new InvalidOperationException("Build error " + workspaceDiagnostic);
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
                    if(standartFormatting)
                    {
                        newDoc = CodeHelper.Formatting(newDoc);
                    }
                    var c = await newDoc.GetTextChangesAsync(old);
                    document = c.Any() ? newDoc : old;
                    project = document.Project;
                }
                // create new document
                else
                {
                    CodeHelper.AddDocument(standartFormatting, project, doc.FileName, code, doc.ProjectFolder.Split('\\'));
                }
            }
            // Apply project changes
            Solution = project.Solution;
            ApplyChanges();
        }

        #endregion

    }
}
