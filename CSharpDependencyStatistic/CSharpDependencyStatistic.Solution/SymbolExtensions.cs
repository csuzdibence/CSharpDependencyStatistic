using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpDependencyStatistic.Solution.Extensions
{
    public static class SymbolExtensions
    {
        public static IEnumerable<ISymbol> GetReferencedSymbols(this ISymbol symbol, SemanticModel semanticModel)
        {
            var references = new List<ISymbol>();

            try
            {
                if (symbol is not IMethodSymbol methodSymbol)
                {
                    return references;
                }

                if (methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is MethodDeclarationSyntax syntax)
                {
                    var referencedIdentifiers = syntax.DescendantNodes().OfType<IdentifierNameSyntax>();

                    foreach (var identifier in referencedIdentifiers)
                    {
                        var referencedSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                        if (referencedSymbol != null)
                        {
                            references.Add(referencedSymbol);
                        }
                    }
                }
            }
            catch
            {
            }

            return references;
        }
    }
}
