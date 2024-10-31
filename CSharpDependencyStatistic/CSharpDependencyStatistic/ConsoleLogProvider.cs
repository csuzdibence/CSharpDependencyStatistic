using Microsoft.Extensions.Logging;

namespace CSharpDependencyStatistic
{
    internal class ConsoleLogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
