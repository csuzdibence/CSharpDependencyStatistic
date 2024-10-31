using CSharpDependencyStatistic.Statistic;

namespace CSharpDependencyStatistic.Plot
{
    public class DependencyStatisticPlotter
    {
        private const int PlotWidth = 1600;
        private const int PlotHeight = 1200;
        private const string PlotTitle = "Abstractness vs. Instability";
        private const string PlotXLabel = "Abstractness (A)";
        private const string PlotYLabel = "Instability (I)";
        private const int LabelFontSize = 6;

        public void PlotGraph(List<DependencyStatistic> dependencyStatistics, string pngFileName)
        {
            var plt = new ScottPlot.Plot();
            plt.Add.ScatterPoints(dependencyStatistics.Select(x => x.Abstractness).ToArray(), dependencyStatistics.Select(x => x.Instability).ToArray());

            foreach (var statistic in dependencyStatistics)
            {
                var text = plt.Add.Text(statistic.ProjectName, statistic.Abstractness, statistic.Instability);
                text.LabelFontSize = LabelFontSize;
            }

            plt.Title(PlotTitle);
            plt.XLabel(PlotXLabel);
            plt.YLabel(PlotYLabel);
            plt.Axes.SetLimits(0, 1, 0, 1);
            plt.Axes.AutoScaleX();
            plt.Axes.AutoScaleY();
            plt.Save(pngFileName, PlotWidth, PlotHeight);
        }
    }
}
