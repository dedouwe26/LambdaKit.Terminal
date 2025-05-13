using System.Text;

namespace LambdaKit.Terminal.Backend;

/// <summary>
/// Represents an interface of common methods of a terminal using ANSI.
/// </summary>
public abstract class TerminalBackend : ITerminalBackend {
    #region Abstractions
    /// <inheritdoc/>
    public abstract TextReader StandardInput { get; }
    /// <inheritdoc/>
    public abstract TextWriter StandardOutput { get; }
    /// <inheritdoc/>
    public abstract TextWriter StandardError { get; }

    /// <inheritdoc/>
    public abstract Encoding InputEncoding { get; set; }
    /// <inheritdoc/>
    public abstract Encoding OutputEncoding { get; set; }
    /// <inheritdoc/>
    public abstract Encoding ErrorEncoding { get; set; }

    /// <inheritdoc/>
    public abstract (uint Width, uint Height) Size { get; set; }

    /// <summary>
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor position.</returns>
    public abstract (int x, int y) GetCursorPosition();

    /// <inheritdoc/>
    public abstract bool BlockCancelKey { get; set; }
    /// <inheritdoc/>
    public abstract bool ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control);
    
    /// <inheritdoc/>
    public abstract void Dispose();
    #endregion

    #region Writing
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public virtual TerminalBackend Write(object? text, Style? style = null, bool postReset = true) {
        StandardOutput.Write((style ?? new Style()).ToANSI()+text?.ToString()+(postReset ? ANSI.SGR.BuildedResetAll : ""));
        return this;
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public virtual TerminalBackend WriteLine(object? text, Style? style = null, bool postReset = true) {
        StandardOutput.WriteLine((style ?? new Style()).ToANSI()+text?.ToString()+(postReset ? ANSI.SGR.BuildedResetAll : ""));
        return this;
    }
    /// <summary>
    /// Writes a line to the terminal, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use.</param>
    public TerminalBackend WriteLine(Style? style = null) {
        WriteLine(null, style);
        return this;
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <param name="text">The text the error output stream.</param>
    /// <param name="style">The style to use (defto write toault: with red foreground).</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public virtual TerminalBackend WriteErrorLine(object? text, Style? style = null, bool postReset = true) {
        StandardError.WriteLine((style ?? new Style {ForegroundColor = (StandardColor)StandardColor.Colors.Red}).ToANSI()+text?.ToString()+(postReset ? ANSI.SGR.BuildedResetAll : ""));
        return this;
    }
    /// <summary>
    /// Writes a line to the error stream, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use (default: with red foreground).</param>
    public TerminalBackend WriteErrorLine(Style? style = null) {
        WriteErrorLine(null, style);
        return this;
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <param name="text">The text to write to the error output stream.</param>
    /// <param name="style">The style to use (default: with red foreground).</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public virtual TerminalBackend WriteError(object? text, Style? style = null, bool postReset = true ) {
        StandardError.Write((style ?? new Style {ForegroundColor = (StandardColor)StandardColor.Colors.Red}).ToANSI()+text?.ToString()+(postReset ? ANSI.SGR.BuildedResetAll : ""));
        return this;
    }
    /// <summary>
    /// Sets the cursor to that position.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual TerminalBackend Goto((int x, int y) pos) {
        if (pos.x >= Size.Width || pos.x < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos x is higher than the width or is lower than 0."); }
        if (pos.y >= Size.Height || pos.y < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos y is higher than the height or is lower than 0."); }
        StandardOutput.Write(ANSI.CUP((uint)pos.x, (uint)pos.y));
        return this;
    }
    /// <summary>
    /// Sets the cursor to that position in the error stream.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual TerminalBackend GotoError((int x, int y) pos) {
        if (pos.x >= Size.Width || pos.x < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos x is higher than the width or is lower than 0."); }
        if (pos.y >= Size.Height || pos.y < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos y is higher than the height or is lower than 0."); }
        StandardError.Write(ANSI.CUP((uint)pos.x, (uint)pos.y));
        return this;
    }
    #endregion

    #region Reading
    /// <summary>
    /// Reads one character from the input stream.
    /// </summary>
    /// <returns>The character that has been read (-1 if everything has been read).</returns>
    public virtual int Read() {
        return StandardInput.Read();
    }
    /// <summary>
    /// Reads a line from the input stream.
    /// </summary>
    /// <returns>The line that has been read (null if everything has been read).</returns>
    public virtual string? ReadLine() {
        return StandardInput.ReadLine();
    }
    
    #region Reading - Keys
    /// <summary>
    /// Waits until a key is pressed.
    /// </summary>
    public virtual void WaitForKeyPress() {
        ReadKey(out _, out _, out _, out _, out _);
    }
    /// <summary>
    /// An event for when a key is pressed.
    /// </summary>
    public event KeyPressCallback? OnKeyPress;
    /// <summary>
    /// The thread that is running <see cref="ListenForKeysMethod"/>.
    /// </summary>
    protected Thread? listenForKeysThread;
    /// <summary>
    /// If this backend is currently listening to keys.
    /// </summary>
    protected bool listenForKeys = false;
    /// <summary>
    /// If it should listen for keys.
    /// </summary>
    public virtual bool ListenForKeys {set {
        if (value && (!listenForKeys)) {
            listenForKeys = value;
            listenForKeysThread = new Thread(ListenForKeysMethod);
            listenForKeysThread.Start();
        } else {
            listenForKeys = value;
        }
    } get {
        return listenForKeys;
    }}
    /// <summary>
    /// Method in new thread that should call <see cref="OnKeyPress"/> when a key is pressed.
    /// </summary>
    protected virtual void ListenForKeysMethod() {
        while (listenForKeys) {
            ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control);
            OnKeyPress?.Invoke(key, keyChar, alt, shift, control);
        }
    }
    #endregion

    #endregion

    #region Cursor
    private bool cursorHidden = false;
    /// <inheritdoc/>
    public virtual bool HideCursor { get => cursorHidden; set {
        StandardOutput.Write(value ? ANSI.CursorInvisible : ANSI.CursorVisible);
        cursorHidden = value;
    } }
    /// <inheritdoc/>
    public virtual (int x, int y) CursorPosition { get => GetCursorPosition(); set {
        Goto(value);
    } }
    #endregion

    #region Clearing
    /// <summary>
    /// Clears everything, including scrollback buffer (XTerm). 
    /// </summary>
    public TerminalBackend ClearAll() {
        StandardOutput.Write(ANSI.EDA);
        return this;
    }
    /// <summary>
    /// Clears (resets) the whole screen.
    /// </summary>
    public TerminalBackend ClearScreen() {
        StandardOutput.Write(ANSI.EDS);
        return this;
    }
    /// <summary>
    /// Clears screen from the position to end of the screen. Moves cursor to that position.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public TerminalBackend ClearFrom((int x, int y) pos) {
        Goto(pos);
        StandardOutput.Write(ANSI.EDCE);
        return this;
    }
    /// <summary>
    /// Clears a line. Moves cursor to that line.
    /// </summary>
    /// <param name="line">The y-axis of the line.</param>
    public TerminalBackend ClearLine(int line) {
        Goto((0, line));
        StandardOutput.Write(ANSI.ELA);
        return this;
    }
    /// <summary>
    /// Clears the line from the position to the end of the line. Moves cursor to that position.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public TerminalBackend ClearLineFrom((int x, int y) pos) {
        Goto(pos);
        StandardOutput.Write(ANSI.ELCE);
        return this;
    }
    #endregion
}