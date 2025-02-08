using Microsoft.Extensions.Logging;

namespace Harmonize.Log;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {formatter(state, exception)}";

        lock (_lock)
        {
            File.AppendAllText(_filePath, logRecord + Environment.NewLine);
        }
    }
}
