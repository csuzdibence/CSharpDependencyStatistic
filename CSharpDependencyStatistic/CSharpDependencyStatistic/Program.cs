using CSharpDependencyStatistic.Plot;
using CSharpDependencyStatistic.Solution;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("You did not give in a solution file path!");
            return;
        }

        string solutionPath = args[0];
        if (!File.Exists(solutionPath))
        {
            Console.WriteLine("Solution file does not exist!");
            return;
        }

        var serviceProvider = ConfigureServices();
        var solutionDependencyCalculator = serviceProvider.GetService<SolutionDependencyCalculator>();

        var dependencyStatistics = solutionDependencyCalculator.GetSolutionDependencies(solutionPath).Result;
        var jsonContent = JsonSerializer.Serialize(dependencyStatistics.OrderBy(x => x.Distance), new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        string jsonPath = solutionPath + ".json";
        File.WriteAllText(jsonPath, jsonContent);
        StartJsonProcess(jsonPath);
    }

    private static void StartJsonProcess(string jsonPath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = jsonPath,
            UseShellExecute = true
        };
        Process.Start(processStartInfo);
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<SolutionReader>();
        serviceCollection.AddTransient<SolutionDependencyCalculator>();
        serviceCollection.AddTransient<DependencyStatisticPlotter>();
        return serviceCollection.BuildServiceProvider();
    }
}