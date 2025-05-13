using System.Collections;
using System.Collections.ObjectModel;
using LambdaKit.Terminal.Logging.Targets;

namespace LambdaKit.Terminal.Logging;

/// <summary>
/// Represents a logger for the terminal and log files.
/// </summary>
public class Logger : IDisposable, IEquatable<Logger> {
    /// <summary>
    /// The ID of this logger (if it is registered).
    /// </summary>
    public readonly string? ID;
    /// <summary>
    /// The name of this logger.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Checks if this is a sub logger.
    /// </summary>
    public bool IsSubLogger => this is SubLogger;

    /// <summary>
    /// All the sub loggers of this logger.
    /// </summary>
    public ReadOnlyDictionary<string, Logger> SubLoggers { get { return subLoggers.AsReadOnly(); } }

    private readonly Dictionary<string, Logger> subLoggers = [];

    /// <summary>
    /// All the targets.
    /// </summary>
    public volatile List<(ITarget target, bool enabled)> Targets;
    /// <summary>
    /// The current log severity max.
    /// </summary>
    public Severity logLevel = Severity.Info;
    /// <summary>
    /// An event for when something is logged.
    /// </summary>
    public event LogCallback? OnLog;


    /// <summary>
    /// Creates a logger.
    /// </summary>
    /// <param name="id">The optional ID to identify this logger, like 'me.0xDED.MyProject'. It won't register if the ID is null (if this ID is already registered it will throw an <see cref="ArgumentException"/> error).</param>
    /// <param name="name">The name of the logger, used in the log files and terminal.</param>
    /// <param name="severity">The log level of this logger.</param>
    /// <param name="targets">The targets to add and enable (default: <see cref="TerminalTarget"/>, <see cref="FileTarget"/> with path "./latest.log").</param>
    /// <exception cref="ArgumentException"/>
    public Logger(string name = "Logger", string? id = null, Severity severity = Severity.Info, List<ITarget>? targets = null) {
        ID = id;
        Name = name;
        logLevel = severity;
        Targets = targets != null ? targets.Select(target => (target, true)).ToList() : [ (new TerminalTarget(), true), (new FileTarget("./latest.log"), true) ];
        if (id != null) {
            if (!Loggers.Register(this)) {
                throw new ArgumentException("A logger with this ID has already been registered.", nameof(id));
            }
        }
    }

    /// <summary>
    /// Checks if this logger is registered.
    /// </summary>
    /// <returns></returns>
    public bool IsRegistered() {
        return ID is not null;
    }

    /// <summary>
    /// Sets the log level.
    /// </summary>
    /// <param name="maxSeverity">The highest log level to log.</param>
    public void SetLevel(Severity maxSeverity) {
        logLevel = maxSeverity;
    }

    /// <summary>
    /// Checks if this logger has the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>True if it has that target.</returns>
    public bool HasTarget(ITarget target) {
        foreach ((ITarget t, _) in Targets) {
            if (ReferenceEquals(t, target)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if this logger has a target with that type.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <returns>True if it has that target type.</returns>
    public bool HasTarget<T>() where T : ITarget {
        foreach ((ITarget t, _) in Targets) {
            if (t is T) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the target's index.
    /// </summary>
    /// <param name="target">The target which index to get.</param>
    /// <returns>The index of the target or -1 if the target could not be found.</returns>
    public int GetTargetIndex(ITarget target) {
        for (int i = 0; i < Targets.Count; i++) {
            if (ReferenceEquals(Targets[i].target, target)) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the first-matching target's index, which type is <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <returns>The index of the target or -1 if the target could not be found.</returns>
    public int GetTargetIndex<T>() where T : ITarget {
        for (int i = 0; i < Targets.Count; i++) {
            if (Targets[i].target is T) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the target from its index.
    /// </summary>
    /// <param name="index">The target index.</param>
    /// <returns>The target if there is one.</returns>
    public ITarget? GetTarget(int index) {
        try {
            return Targets[index].target;
        } catch (IndexOutOfRangeException) {
            return null;
        }
    }
    /// <summary>
    /// Gets the target from its index or default.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <param name="index">The target index.</param>
    /// <returns>The target if there is one and if it is a <typeparamref name="T"/>.</returns>
    public T? GetTarget<T>(int index) where T : class, ITarget {
        try {
            return Targets[index].target as T;
        } catch (IndexOutOfRangeException) {
            return default;
        }
    }
    /// <summary>
    /// Gets the first target with that type or default.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <returns>The first found target with that type, if there is one and if it is a <typeparamref name="T"/>.</returns>
    public T? GetTarget<T>() where T : ITarget {
        foreach ((ITarget target, _) in Targets) {
            if (target is T t) {
                return t;
            }
        }
        return default;
    }

    /// <summary>
    /// Sets whether a target is enabled.
    /// </summary>
    /// <param name="target">The target to enable or disable.</param>
    /// <param name="enabled">True if it should be enabled.</param>
    /// <returns>True if it could find that target.</returns>
    public bool SetTarget(ITarget target, bool enabled) {
        int index = GetTargetIndex(target);
        if (index == -1) {
            return false;
        }
        Targets[index] = (target, enabled);
        return true;
    }

    /// <summary>
    /// Sets whether the first-matching target is enabled.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <param name="enabled">True if it should be enabled.</param>
    /// <returns>True if it could find a matching target.</returns>
    public bool SetTarget<T>(bool enabled) where T : ITarget {
        int index = GetTargetIndex<T>();
        if (index == -1) {
            return false;
        }
        Targets[index] = (Targets[index].target, enabled);
        return true;
    }

    /// <summary>
    /// Sets whether a target is enabled.
    /// </summary>
    /// <param name="index">The index of the target.</param>
    /// <param name="enabled">True if it should be enabled.</param>
    /// <returns>True if it was a valid index.</returns>
    public bool SetTarget(int index, bool enabled) {
        ITarget? target = GetTarget(index);
        if (target == null) {
            return false;
        }
        try {
            Targets[index] = (target, enabled);
        } catch (IndexOutOfRangeException) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Adds a target.
    /// </summary>
    /// <param name="target">The target to add (e.g. typeof(TerminalTarget)).</param>
    /// <param name="enabled">If it is enabled.</param>
    public void AddTarget(ITarget target, bool enabled = true) {
        Targets.Add((target, enabled));
    }

    /// <summary>
    /// Removes a target at that index.
    /// </summary>
    /// <param name="index">The index of the target to remove.</param>
    /// <returns>True if the target could be found.</returns>
    public bool RemoveTargetAt(int index) {
        try {
            Targets.RemoveAt(index);
            return true;
        } catch (IndexOutOfRangeException) {
            return false;
        }
    }

    /// <summary>
    /// Removes a target.
    /// </summary>
    /// <param name="target">The target to remove.</param>
    /// <returns>True if the target could be found.</returns>
    public bool RemoveTarget(ITarget target) {
        return RemoveTargetAt(GetTargetIndex(target));
    }

    /// <summary>
    /// Removes the first target with that type.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <returns>True if the target could be found.</returns>
    public bool RemoveTarget<T>() where T : ITarget {
        return RemoveTargetAt(GetTargetIndex<T>());
    }

    /// <summary>
    /// Checks if this logger has a sub logger with that ID.
    /// </summary>
    /// <param name="childID">The child ID of the sub logger.</param>
    /// <returns>True if this logger has a sub logger with that ID.</returns>
    public bool HasSubLogger(string childID) {
        return subLoggers.ContainsKey(childID);
    }
    /// <summary>
    /// Checks if this logger has this logger as a sub logger.
    /// </summary>
    /// <param name="logger">The sub logger.</param>
    /// <returns>True if this logger has that sub logger.</returns>
    public bool HasSubLogger(SubLogger logger) {
        return ReferenceEquals(this, logger.ParentLogger);
    }

    /// <summary>
    /// Gets the sub logger from an ID.
    /// </summary>
    /// <remarks>
    /// Please use <see cref="Loggers.Get(string)"/>, if you want to get a logger with that ID.
    /// Or use <see cref="HasSubLogger(string)"/> if you want to know if it has a sub logger with that ID.
    /// </remarks>
    /// <param name="childID">The child ID of the sub logger.</param>
    /// <returns>The sub logger.</returns>
    public Logger? GetSubLogger(string childID) {
        subLoggers.TryGetValue(childID, out Logger? logger);
        return logger;
    }

    private bool handlingUnhandledExceptions;

    private void HandleException(object sender, UnhandledExceptionEventArgs args) {
        Exception e = (Exception) args.ExceptionObject;
        LogException(e, args.IsTerminating ? Severity.Fatal : Severity.Error, true);
    }

    /// <summary>
    /// Logs unhandled exceptions in the current app domain to this logger.
    /// </summary>
    public bool HandleUnhandledExceptions {
        get {
            return handlingUnhandledExceptions;
        }
        set {
            if (value) {
                AppDomain.CurrentDomain.UnhandledException += HandleException;
            } else {
                AppDomain.CurrentDomain.UnhandledException -= HandleException;
            }
            handlingUnhandledExceptions = value;
        }
    }

    /// <summary>
    /// Logs something (<see cref="object.ToString"/>).
    /// </summary>
    /// <param name="severity">The severity of the text.</param>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void Log(Severity severity, object? text) {
        DateTime time = DateTime.Now;
        OnLog?.Invoke(this, text?.ToString()??"", severity, time);
        if (((byte)severity) > ((byte)logLevel)) { return; }
        foreach ((ITarget target, bool enabled) target in Targets) {
            if (target.enabled) {
                target.target.Write(severity, time, this, text);
            }
        }
    }

    /// <summary>
    /// Logs something with Trace severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogTrace(object? text) {
        Log(Severity.Trace, text);
    }
    /// <summary>
    /// Logs something with Debug severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogDebug(object? text) {
        Log(Severity.Debug, text);
    }
    /// <summary>
    /// Logs something with Info severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogInfo(object? text) {
        Log(Severity.Info, text);
    }
    /// <summary>
    /// Logs something with Message severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogMessage(object? text) {
        Log(Severity.Message, text);
    }
    /// <summary>
    /// Logs something with Warning severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogWarning(object? text) {
        Log(Severity.Warning, text);
    }
    /// <summary>
    /// Logs something with Error severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogError(object? text) {
        Log(Severity.Error, text);
    }
    /// <summary>
    /// Logs something with Fatal severity.
    /// </summary>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void LogFatal(object? text) {
        Log(Severity.Fatal, text);
    }

    private static string GetStacktrace(Exception e) {
        string stackTrace = e.StackTrace ?? "   (Unknown)";
        if (e.InnerException != null) {
            Exception inner = e.InnerException;
            string source = inner.Source == null ? "" : $" (in: {inner.Source})";
            return stackTrace + $"\nCaused by {inner.GetType().FullName} : '{inner.Message}'{source}: \n{GetStacktrace(inner)}";
        } else {
            return stackTrace;
        }
    }
    /// <summary>
    /// Logs an exception with a custom format.
    /// </summary>
    /// <param name="e">The exception to log.</param>
    /// <param name="severity">The severity of that exception.</param>
    public void LogException(Exception e, Severity severity = Severity.Error) {
        LogException(e, severity, false);
    }
    private void LogException(Exception e, Severity severity, bool unhandled) {
        string source = e.Source == null ? "" : $" (in: {e.Source})";
        string unhandledStr = unhandled?"Unhandled Exception : " : "";
        string message = $"{unhandledStr}{e.GetType().Name} ({e.GetType().FullName}) : '{e.Message}'\nTrace{source}:\n{GetStacktrace(e)}";
        
        if (e.HelpLink != null) {
            message += "\n\nHelp: "+e.HelpLink;
        }
        
        Log(severity, message);
    }

    /// <summary>
    /// Creates a sub logger.
    /// </summary>
    /// <param name="name">The sub name of the logger.</param>
    /// <param name="id">The sub ID. Full ID will be: parent ID + '.' + subID = child ID).</param>
    /// <param name="shouldRegister">If the sublogger should be registered, if the parent logger is also registered.</param>
    /// <param name="severity">The log level of the new sub logger.</param>
    /// <param name="targets">The targets of the new sub logger (default: Targets of parent).</param>
    /// <returns>The created sub logger.</returns>
    /// <exception cref="ArgumentException" />
    public SubLogger CreateSubLogger(string id, string name = "Sublogger", bool shouldRegister = true, Severity severity = Severity.Info, List<ITarget>? targets = null) {
        SubLogger subLogger = new(this, id, name, (ID != null && shouldRegister) ? ID+'.'+id : null, severity, targets);
        subLoggers.Add(id, subLogger);
        return subLogger;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Disposes all targets, and unregisters this logger.
    /// </remarks>
    public void Dispose() {
        Loggers.UnRegister(this);

        foreach ((ITarget target, _) in Targets) {
            target.Dispose();
        }

        foreach (KeyValuePair<string, Logger> subLogger in subLoggers) {
            subLogger.Value.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    /// 
    public static bool operator ==(Logger? left, Logger? right) {
        if (left is null && right is null) {
            return true;
        } else if (left is null) {
            return false;
        }
        return left.Equals(right);
    }
    /// 
    public static bool operator !=(Logger? left, Logger? right) {
        return !(left == right);
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Checks if the that color is identical to this one.
    /// </remarks>
    public bool Equals(Logger? other) {
        if (other is null) {
            return false;
        }
        if (ReferenceEquals(this, other)) {
            return true;
        }
        if (GetType() != other.GetType()) {
            return false;
        }
        if (ID!=null) {
            return ID == other.ID;
        }
        return false;
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Checks if the that color is identical to this one.
    /// </remarks>
    public override bool Equals(object? obj) {
        return Equals(obj as Logger);
    }
    /// <inheritdoc/>
    public override int GetHashCode() {
        return ID == null ? Name.GetHashCode() ^ subLoggers.GetHashCode() ^ Targets.GetHashCode() : ID.GetHashCode();
    }
}