namespace OxDED.Terminal.Logging.Targets;

/// <summary>
/// A Logger Target for the terminal.
/// </summary>
public class TerminalTarget : FormattedTarget {
    /// <summary>
    /// The out stream to the terminal.
    /// </summary>
    public TextWriter Out;
    /// <summary>
    /// The error stream to the terminal.
    /// </summary>
    public TextWriter Error;
    /// <summary>
    /// The format to use for writing to the terminal (0: name see <see cref="FormattedTarget.NameFormat"/>, 1: logger ID, 2: time, 3: severity, 4: message, 5: color ANSI).
    /// </summary>
    /// <remarks>
    /// Default:
    /// <c>{5}[{0}][{2}][BOLD{3}RESETBOLD]: {4}RESETALL</c>
    /// </remarks>
    public new string Format = "{5}[{0}][{2}]["+ANSI.SGR.Build(ANSI.SGR.BOLD)+"{3}"+ANSI.SGR.Build(ANSI.SGR.RESETINTENSITY)+"]: {4}"+ANSI.SGR.BuildedResetAll;
    /// <summary>
    /// The colors of the severities (index: 0: Fatal, 1: Error, 2: Warning, 3: Message, 4: Info, 5: Debug, 6: Trace).
    /// </summary>
    public readonly List<IColor> SeverityColors = [
        (StandardColor)StandardColor.Colors.BrightRed, (StandardColor)StandardColor.Colors.Red, (StandardColor)StandardColor.Colors.Yellow, (StandardColor)StandardColor.Colors.BrightWhite, (StandardColor)StandardColor.Colors.White, RGBColor.Orange, (StandardColor)StandardColor.Colors.Green
    ];
    /// <summary>
    /// Creates a target that targets the terminal.
    /// </summary>
    /// <param name="format">The format to write to the terminal (default, more info: <see cref="Format"/>).</param>
    /// <param name="terminalOut">The out stream (default: <see cref="Terminal.Out"/>).</param>
    /// <param name="terminalError">The error stream (default: <see cref="Terminal.Error"/>).</param>
    public TerminalTarget(string? format = null, TextWriter? terminalOut = null, TextWriter? terminalError = null) {
        if (format != null) {
            Format = format;
        }
        Out = terminalOut ?? Terminal.Out;
        Error = terminalError ?? Terminal.Error;
    }
    /// <inheritdoc/>
    public override void Dispose() {
        GC.SuppressFinalize(this);
    }

    private string GetText(Logger logger, DateTime time, Severity severity, string text, string color) {
        return string.Format(Format, GetName(logger), logger.ID, time.ToString(), severity.ToString(), text, color);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Writes a line.
    /// </remarks>
    public override void Write(Severity severity, DateTime time, Logger logger, object? text) {
        if (((byte)severity) < 2) {
            Error.WriteLine(GetText(logger, time, severity, text?.ToString() ?? "(Null)", SeverityColors[(byte)severity].ToForegroundANSI()));
        } else {
            Out.WriteLine(GetText(logger, time, severity, text?.ToString() ?? "(Null)", SeverityColors[(byte)severity].ToForegroundANSI()));
        }
        
    }
}