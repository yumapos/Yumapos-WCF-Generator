using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

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
        }

        public Solution Solution { get; set; }
        public MSBuildWorkspace MsBuildWorkspace { get; private set; }
    }
}
