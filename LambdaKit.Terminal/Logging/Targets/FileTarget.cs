namespace LambdaKit.Terminal.Logging.Targets;

/// <summary>
/// A Logger Target for a log file.
/// </summary>
public class FileTarget : FormattedTarget {
    /// <summary>
    /// The output stream to the file.
    /// </summary>
    public readonly TextWriter FileOut;
    
    /// <summary>
    /// Creates a target that targets a log file.
    /// </summary>
    /// <param name="path">The path to the log file (file doesn't need to exist).</param>
    /// <param name="format">The format to use for writing to a file ().</param>
    public FileTarget(string path, string? format = null) {
        if (format != null) {
            Format = format;
        }
        FileOut = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Writes a line.
    /// </remarks>
    public override void Write(Severity severity, DateTime time, Logger logger, object? text) {
        FileOut.WriteLine(GetText(logger, time, severity, text?.ToString() ?? "(Null)"));
    }
    /// <inheritdoc/>
    public override void Dispose() {
        FileOut.Close();
        GC.SuppressFinalize(this);
    }
}