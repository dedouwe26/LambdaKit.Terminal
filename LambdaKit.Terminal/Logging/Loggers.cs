namespace LambdaKit.Terminal.Logging;

/// <summary>
/// A register for all loggers.
/// </summary>
public static class Loggers {
    private static readonly Dictionary<string, Logger> registeredLoggers = [];
    /// <summary>
    /// Registers a logger.
    /// </summary>
    /// <param name="logger">The logger to register.</param>
    /// <returns>False if there already is a logger with that ID or if the ID of that logger is null.</returns>
    public static bool Register(Logger logger) {
        if (logger.ID == null) return false;
        if (registeredLoggers.ContainsKey(logger.ID)) { return false; }
        registeredLoggers.Add(logger.ID, logger);
        return true;
    }
    /// <summary>
    /// Unregisters a logger.
    /// </summary>
    /// <param name="ID">The ID of the logger to unregister.</param>
    /// <returns>True if it was successful, false if there is no logger with that ID.</returns>
    public static bool UnRegister(string ID) {
        return registeredLoggers.Remove(ID);
    }
    /// <summary>
    /// Unregisters a logger.
    /// </summary>
    /// <param name="logger">The logger to unregister.</param>
    /// <returns>True if it was successful, false if that logger isn't registered.</returns>
    public static bool UnRegister(Logger logger) {
        if (logger.ID == null) return false;
        return registeredLoggers.Remove(logger.ID);
    }
    /// <summary>
    /// Gets the logger to the corresponding ID if there is one.
    /// </summary>
    /// <param name="ID">The ID of the logger.</param>
    /// <returns>The logger with that ID (if there is one registered).</returns>
    public static Logger? Get(string ID) {
        registeredLoggers.TryGetValue(ID, out Logger? logger);
        return logger;
    }
}