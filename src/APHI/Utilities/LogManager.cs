using System;
using System.IO;

namespace APHI.Utilities;

/// <summary>
/// A simple, thread-safe logger for the APHI extension.
/// </summary>
public class LogManager
{
    private readonly string _logDirectory;
    private readonly string _logFilePath;
    private readonly object _lockObj = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="LogManager"/> class.
    /// </summary>
    public LogManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _logDirectory = Path.Combine(appData, "APHI", "Logs");
        
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }

        _logFilePath = Path.Combine(_logDirectory, $"APHI_{DateTime.Now:yyyyMMdd}.log");
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    public void LogInfo(string message)
    {
        WriteLog("INFO", message);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public void LogWarning(string message)
    {
        WriteLog("WARN", message);
    }

    /// <summary>
    /// Logs an error message with optional exception details.
    /// </summary>
    public void LogError(string message, Exception? ex = null)
    {
        if (ex != null)
        {
            WriteLog("ERROR", $"{message}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
        else
        {
            WriteLog("ERROR", message);
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    public void LogDebug(string message)
    {
#if DEBUG
        WriteLog("DEBUG", message);
#endif
    }

    /// <summary>
    /// Exports the current log file to a specified destination.
    /// </summary>
    public void ExportLog(string destinationPath)
    {
        lock (_lockObj)
        {
            if (File.Exists(_logFilePath))
            {
                File.Copy(_logFilePath, destinationPath, true);
            }
        }
    }

    private void WriteLog(string severity, string message)
    {
        try
        {
            lock (_lockObj)
            {
                using var writer = new StreamWriter(_logFilePath, true);
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{severity}] {message}");
            }
        }
        catch
        {
            // Ensure logging failures don't crash the application
            System.Diagnostics.Debug.WriteLine($"Failed to write to log: {message}");
        }
    }
}
