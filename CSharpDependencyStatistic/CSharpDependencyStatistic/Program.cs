using CSharpDependencyStatistic.Plot;
using CSharpDependencyStatistic.Solution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"{nameof(CSharpDependencyStatistic)} application initialised!");
        if (args.Length == 0)
        {
            Console.WriteLine("You did not give in a solution file path!");
            return;
        }

        string solutionPath = args[0];
        if (!Path.IsPathRooted(solutionPath))
        {
            string baseDirectory = Environment.GetEnvironmentVariable("SOLUTION_BASE_DIRECTORY") ?? Environment.CurrentDirectory;
            solutionPath = Path.Combine(baseDirectory, solutionPath);
        }

        if (!File.Exists(solutionPath))
        {
            Console.WriteLine("Solution file does not exist!");
            return;
        }

        var serviceProvider = ConfigureServices();
        var solutionDependencyCalculator = serviceProvider.GetService<SolutionDependencyCalculator>();
        var plotter = serviceProvider.GetService<DependencyStatisticPlotter>();

        Console.WriteLine("Solution dependencies are started to get analysed...");
        var dependencyStatistics = solutionDependencyCalculator.GetSolutionDependencies(solutionPath).Result;

        string pngFilePath = solutionPath + ".png";
        plotter.PlotGraph(dependencyStatistics, pngFilePath);

        var jsonContent = JsonSerializer.Serialize(dependencyStatistics.OrderBy(x => x.Distance), new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        string jsonPath = solutionPath + ".json";
        File.WriteAllText(jsonPath, jsonContent);
        StartProcess(jsonPath);
    }

    private static void StartProcess(string filePath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        };
        Process.Start(processStartInfo);
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder =>
         {
             builder.AddConsole();
             builder.SetMinimumLevel(LogLevel.Information);
         });
        serviceCollection.AddTransient<SolutionReader>();
        serviceCollection.AddTransient<SolutionDependencyCalculator>();
        serviceCollection.AddTransient<DependencyStatisticPlotter>();
        return serviceCollection.BuildServiceProvider();
    }
}