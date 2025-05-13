namespace LambdaKit.Terminal.Logging;

/// <summary>
/// Represents a target for logger outputs.
/// </summary>
public interface ITarget : IDisposable {
    /// <summary>
    /// The method to write to output.
    /// </summary>
    /// <param name="severity">The severity of the message.</param>
    /// <param name="time">The time when it has been logged.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="text">The text to write (<see cref="object.ToString"/>).</param>
    public void Write(Severity severity, DateTime time, Logger logger, object? text);
}