using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

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

        public RepositoryGeneratorWorkSpace(string solutionPath, string projectName)
        {
            _workspace = MSBuildWorkspace.Create();
            _workspace.DocumentOpened += (sender, args) => Console.WriteLine(args.Document.Name);
            Solution = _workspace.OpenSolutionAsync(solutionPath).Result;

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
        public void ApplyChanges()
        {
            var project = Project;
            
            // Remove old file
            foreach (var doc in _filesToCreation)
            {
                var old = project.Documents.FirstOrDefault(x => x.FilePath.EndsWith(doc.ProjectFolder + "\\" + doc.FileName));
                if (old != null)
                {
                    project = project.RemoveDocument(old.Id);
                }
            }

            // Add new file
            foreach (var task in _filesToCreation)
            {
                project = project.AddDocument(task.FileName, task.SrcText, task.ProjectFolder.Split('\\')).Project;
            }
            // Apply project changes
            _workspace.TryApplyChanges(project.Solution);
        }

        #endregion

        #region Private

        private struct SrcFile
        {
            public readonly string FileName;
            public readonly string ProjectFolder;
            public readonly string SrcText;

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