using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MalumMenu;

public sealed class WiniLogListener : ILogListener
{
    private const long MaxBytes = 2 * 1024 * 1024;
    private static readonly TimeSpan ReportInterval = TimeSpan.FromSeconds(5);

    private static WiniLogListener _instance;

    private readonly string _path;
    private readonly object _sync = new object();
    private readonly Timer _timer;

    private string _currentMessage;
    private string _currentSource;
    private int _repeats;
    private int _reportedRepeats;
    private long _written;
    private bool _truncated;

    private WiniLogListener(string path)
    {
        _path = path;
        _timer = new Timer(Report, null, ReportInterval, ReportInterval);
    }

    public LogLevel LogLevelFilter => LogLevel.All;

    public static void Attach(string path)
    {
        var listener = new WiniLogListener(path);
        _instance = listener;

        if (listener.AttachBepInEx())
        {
            listener.WriteEntry("wini.logs", "attached to BepInEx log listeners", 1);
            return;
        }

        if (listener.AttachUnity())
        {
            listener.WriteEntry("wini.logs", "attached to Application.logMessageReceived", 1);
            return;
        }

        listener.WriteEntry("wini.logs", "no log hook available, only manually recorded exceptions", 1);
    }

    public static void Record(string source, Exception exception)
    {
        if (exception == null)
        {
            return;
        }

        try
        {
            _instance?.Enqueue(source, exception.ToString());
        }
        catch
        {
        }
    }

    private bool AttachBepInEx()
    {
        try
        {
            var listeners = typeof(Logger)
                .GetProperty("Listeners", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null) as ICollection<ILogListener>;

            if (listeners == null)
            {
                return false;
            }

            listeners.Add(this);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool AttachUnity()
    {
        try
        {
            var logEvent = typeof(Application).GetEvent("logMessageReceived", BindingFlags.Public | BindingFlags.Static);
            var logMethod = typeof(WiniLogListener).GetMethod(nameof(LogUnity), BindingFlags.Instance | BindingFlags.NonPublic);

            if (logEvent == null || logMethod == null)
            {
                return false;
            }

            logEvent.AddEventHandler(null, Delegate.CreateDelegate(logEvent.EventHandlerType, this, logMethod));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _timer.Dispose();

        lock (_sync)
        {
            WriteCurrent();
        }
    }

    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        string source = eventArgs.Source?.SourceName ?? "unknown";

        Enqueue(source, Format(eventArgs.Data));
    }

    private void LogUnity(string condition, string stackTrace, LogType type)
    {
        string message = string.IsNullOrEmpty(stackTrace)
            ? condition
            : condition + Environment.NewLine + stackTrace;

        Enqueue("Unity", message);
    }

    private void Enqueue(string source, string message)
    {
        lock (_sync)
        {
            if (string.Equals(_currentMessage, message, StringComparison.Ordinal) &&
                string.Equals(_currentSource, source, StringComparison.Ordinal))
            {
                _repeats++;
                return;
            }

            WriteCurrent();

            _currentSource = source;
            _currentMessage = message;
            _repeats = 1;
            _reportedRepeats = 1;

            WriteEntry(source, message, 1);
        }
    }

    private void Report(object state)
    {
        lock (_sync)
        {
            if (_currentMessage == null || _repeats <= _reportedRepeats)
            {
                return;
            }

            _reportedRepeats = _repeats;

            WriteEntry(_currentSource, _currentMessage + " (still repeating)", _repeats);
        }
    }

    private void WriteCurrent()
    {
        if (_currentMessage == null)
        {
            return;
        }

        WriteEntry(_currentSource, _currentMessage, _repeats);

        _currentMessage = null;
        _currentSource = null;
        _repeats = 0;
        _reportedRepeats = 0;
    }

    private void WriteEntry(string source, string message, int repeats)
    {
        string count = repeats > 1 ? $" (repeated {repeats} times)" : string.Empty;

        Write($"[{Timestamp()}] {source}{count}:{Environment.NewLine}{message}{Environment.NewLine}");
    }

    private void Write(string text)
    {
        if (_truncated)
        {
            return;
        }

        try
        {
            int bytes = Encoding.UTF8.GetByteCount(text);
            if (_written + bytes > MaxBytes)
            {
                _truncated = true;
                Append($"[{Timestamp()}] wini.logs limit of {MaxBytes} bytes reached, further output dropped{Environment.NewLine}");
                return;
            }

            File.AppendAllText(_path, text);
            _written += bytes;
        }
        catch
        {
            _truncated = true;
        }
    }

    private void Append(string text)
    {
        try
        {
            File.AppendAllText(_path, text);
        }
        catch
        {
        }
    }

    private static string Format(object data)
    {
        if (data == null)
        {
            return string.Empty;
        }

        if (data is Exception exception)
        {
            return exception.ToString();
        }

        return data.ToString() ?? string.Empty;
    }

    private static string Timestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
    }
}