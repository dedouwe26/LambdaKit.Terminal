
namespace LambdaKit.Terminal.Logging.Targets;

/// <summary>
/// Represents a target with formattable logs.
/// </summary>
public abstract class FormattedTarget : ITarget {
    /// <summary>
    /// The format to use for writing to the terminal (0: logger name, 1: logger ID, 2: time, 3: severity, 4: message).
    /// </summary>
    /// <remarks>
    /// Default:
    /// <c>[{0}][{2}][{3}]: {4}</c>
    /// </remarks>
    public string Format = "[{0}][{2}][{3}]: {4}";

    /// <summary>
    /// The format to use for creating names (0: parent name, 1: own name). <para/>
    /// Example for default:
    /// <c>{{App}: Sublogger}: sub-sublogger</c>
    /// </summary>
    /// <remarks>
    /// default:
    /// <c>{0}: {1}</c>
    /// </remarks>
    public string NameFormat = "{0}: {1}";

    /// <summary>
    /// Generates the name of a logger with a format.
    /// </summary>
    /// <param name="logger">The logger which name to generate.</param>
    /// <returns>The generated name.</returns>
    protected string GetName(Logger logger) {
        if (!logger.IsSubLogger) {
            return logger.Name;
        } else {
            return string.Format(NameFormat, GetName((logger as SubLogger)!.ParentLogger), logger.Name);
        }
    }

    /// <summary>
    /// Generates the log with a format.
    /// </summary>
    /// <param name="logger">The logger which name and ID to use.</param>
    /// <param name="time">The time of the log.</param>
    /// <param name="severity">The severity of the log.</param>
    /// <param name="text">The text of the log.</param>
    /// <returns>The generated log.</returns>
    protected string GetText(Logger logger, DateTime time, Severity severity, string text) {
        return string.Format(Format, GetName(logger), logger.ID, time.ToString(), severity.ToString(), text);
    }

    /// <inheritdoc/>
    public abstract void Write(Severity severity, DateTime time, Logger logger, object? text);

    /// <inheritdoc/>
    public abstract void Dispose();
}