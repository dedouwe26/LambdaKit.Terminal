using System.Text;

namespace OxDED.Terminal.Backend;

/// <summary>
/// Represents an interface of common methods of a terminal.
/// </summary>
public interface ITerminalBackend : IDisposable {
    /// <summary>
    /// The data stream for reading from the terminal.
    /// </summary>
    public TextReader StandardInput { get; }
    /// <summary>
    /// The data stream for writing to the terminal.
    /// </summary>
    public TextWriter StandardOutput { get; }
    /// <summary>
    /// The data stream for writing errors to the terminal.
    /// </summary>
    public TextWriter StandardError { get; }
    /// <summary>
    /// The encoding used for the <see cref="StandardInput"/> stream (default: UTF-8).
    /// </summary>
    public Encoding InputEncoding { get; set; }
    /// <summary>
    /// The encoding used for the <see cref="StandardOutput"/> stream (default: UTF-8).
    /// </summary>
    public Encoding OutputEncoding { get; set; }
    /// <summary>
    /// The encoding used for the <see cref="StandardError"/> stream (default: UTF-8).
    /// </summary>
    public Encoding ErrorEncoding { get; set; }
    /// <summary>
    /// The width and the height (in characters) of the terminal.
    /// </summary>
    public (uint Width, uint Height) Size { get; set; }
    /// <summary>
    /// Hides or shows terminal cursor.
    /// </summary>
    public bool HideCursor { get; set; }
    /// <summary>
    /// Gets or sets the cursor position.
    /// </summary>
    public (int x, int y) CursorPosition { get; set; }
    /// <summary>
    /// If it should block CTRL + C.
    /// </summary>
    public bool BlockCancelKey { get; set; }
    
    /// <summary>
    /// Reads a key from the terminal.
    /// </summary>
    /// <param name="key">The read key.</param>
    /// <param name="keyChar">The key character representing that key.</param>
    /// <param name="alt">True if the alt key was pressed.</param>
    /// <param name="shift">True if the shift key was pressed.</param>
    /// <param name="control">True if the control key was pressed.</param>
    /// <returns>True if the key was read successfully.</returns>
    public bool ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control);
}
