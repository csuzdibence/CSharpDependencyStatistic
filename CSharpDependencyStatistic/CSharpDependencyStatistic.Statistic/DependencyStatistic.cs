namespace CSharpDependencyStatistic.Statistic
{
    public class DependencyStatistic
    {
        public DependencyStatistic(string projectName, int afferentCoupling, int efferentCoupling, int totalTypes, int abstractTypes)
        {
            Abstractness = CalculateAbstractness(totalTypes, abstractTypes);
            Instability = CalculateInstability(afferentCoupling, efferentCoupling);
            ProjectName = projectName;
            Distance = (Math.Abs(Abstractness + Instability - 1)) / Math.Sqrt(2);
        }

        public double Abstractness { get; }

        public double Distance { get; }

        public double Instability { get; }

        public string ProjectName { get; }

        private double CalculateAbstractness(int totalTypes, int abstractTypes)
        {
            return totalTypes == 0 ? 0 : (double)abstractTypes / totalTypes;
        }

        private double CalculateInstability(int afferentCoupling, int efferentCoupling)
        {
            return (efferentCoupling + afferentCoupling) == 0 ? 0 : (double)efferentCoupling / (efferentCoupling + afferentCoupling);
        }
    }
}
