using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace WCFGenerator.Common
{
    public class GeneratorWorkspace
    {
        public GeneratorWorkspace(string absoluteSlnPath)
        {
            MsBuildWorkspace = MSBuildWorkspace.Create();
            Solution = MsBuildWorkspace.OpenSolutionAsync(absoluteSlnPath).Result;
        }

        public Solution Solution { get; set; }
        public MSBuildWorkspace MsBuildWorkspace { get; private set; }

        public bool ApplyChanges()
        {
            if (MsBuildWorkspace.CanApplyChange(ApplyChangesKind.AddDocument) || MsBuildWorkspace.CanApplyChange(ApplyChangesKind.ChangeDocument))
                return MsBuildWorkspace.TryApplyChanges(Solution);
            return true;
        }

        public void CloseSolution()
        {
            ApplyChanges();
            MsBuildWorkspace.CloseSolution();
        }
    }
}
