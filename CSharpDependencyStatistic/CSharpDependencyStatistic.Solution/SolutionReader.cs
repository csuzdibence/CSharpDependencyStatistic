using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpDependencyStatistic.Solution
{
    public class SolutionReader
    {
        public async Task<Microsoft.CodeAnalysis.Solution> GetSolution(string solutionPath)
        {
            using var workspace = MSBuildWorkspace.Create();
            return await workspace.OpenSolutionAsync(solutionPath);
        }
    }
}
