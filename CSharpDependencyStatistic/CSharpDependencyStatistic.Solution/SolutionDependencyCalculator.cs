using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpDependencyStatistic.Solution.Extensions;
using CSharpDependencyStatistic.Statistic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace CSharpDependencyStatistic.Solution
{
    public class SolutionDependencyCalculator
    {
        private readonly SolutionReader solutionReader;
        private readonly ILogger<SolutionDependencyCalculator> logger;

        public SolutionDependencyCalculator(SolutionReader solutionReader, ILogger<SolutionDependencyCalculator> logger)
        {
            this.solutionReader = solutionReader;
            this.logger = logger;
        }

        public async Task<List<DependencyStatistic>> GetSolutionDependencies(string solutionPath)
        {
            ConcurrentBag<DependencyStatistic> statistics = new ConcurrentBag<DependencyStatistic>();
            var solution = await solutionReader.GetSolution(solutionPath);

            Parallel.ForEach(solution.Projects, project =>
            {
                statistics.Add(CollectDependencyStatistic(solution, project));
            });

            return statistics.ToList();
        }

        private DependencyStatistic CollectDependencyStatistic(Microsoft.CodeAnalysis.Solution solution, Project project)
        {
            int totalTypes = 0;
            int abstractTypes = 0;
            int afferentCoupling = 0;
            int efferentCoupling = 0;

            foreach (var document in project.Documents)
            {
                var root = document.GetSyntaxRootAsync().Result;
                var semanticModel = document.GetSemanticModelAsync().Result;
                var typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
                totalTypes += typeDeclarations.Count();
                foreach (var typeDecl in typeDeclarations)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;

                    if (symbol != null && (symbol.TypeKind == TypeKind.Interface || symbol.IsAbstract))
                    {
                        abstractTypes++;
                    }

                    var dependencies = symbol?.GetMembers().SelectMany(m => m.GetReferencedSymbols(semanticModel));
                    foreach (var dependency in dependencies)
                    {
                        try
                        {
                            if (dependency != null && dependency.ContainingAssembly.Name != project.AssemblyName)
                            {
                                efferentCoupling++;
                            }
                        }
                        catch
                        {
                        }
                    }
                }

            }

            afferentCoupling = CalculateAfferentCoupling(project, solution);
            var statistic = new DependencyStatistic(project.Name, afferentCoupling, efferentCoupling, totalTypes, abstractTypes);
            logger.LogInformation(statistic.ToString());
            return statistic;
        }

        private int CalculateAfferentCoupling(Project project, Microsoft.CodeAnalysis.Solution solution)
        {
            int afferentCoupling = 0;
            foreach (var otherProject in solution.Projects)
            {
                if (otherProject == project) continue;

                foreach (var document in otherProject.Documents)
                {
                    var root = document.GetSyntaxRootAsync().Result;
                    var semanticModel = document.GetSemanticModelAsync().Result;
                    var references = root.DescendantNodes().OfType<IdentifierNameSyntax>()
                                         .Select(id => semanticModel.GetSymbolInfo(id).Symbol?.ContainingAssembly?.Name);

                    if (references.Contains(project.AssemblyName))
                    {
                        afferentCoupling++;
                    }
                }
            }
            return afferentCoupling;
        }
    }
}
