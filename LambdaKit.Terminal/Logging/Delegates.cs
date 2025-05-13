namespace LambdaKit.Terminal.Logging;

/// <summary>
/// A callback delegate for <see cref="Logger.OnLog"/>
/// </summary>
public delegate void LogCallback(Logger logger, string message, Severity severity, DateTime time);