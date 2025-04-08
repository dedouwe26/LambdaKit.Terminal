using System.Runtime.InteropServices;
using System.Text;
using OxDED.Terminal.Backend;
using OxDED.Terminal.Backend.Window;

namespace OxDED.Terminal;

/// <summary>
/// An delegate for the key press event.
/// </summary>
/// <param name="key">The key that is pressed.</param>
/// <param name="keyChar">The corresponding char of the key (shift is used).</param>
/// <param name="alt">If the alt key was pressed.</param>
/// <param name="shift">If the shift key was pressed.</param>
/// <param name="control">If the control key was pressed.</param>
public delegate void KeyPressCallback(ConsoleKey key, char keyChar, bool alt, bool shift, bool control);

/// <summary>
/// Handles all the terminal stuff.
/// </summary>
public static class Terminal {
    private static ITerminalBackend backend;
    static Terminal() {
        backend = CreateBackend();
        OutputEncoding = Encoding.UTF8;
        InputEncoding = Encoding.UTF8;
        BlockCancelKey = false;
    }

    /// <summary>
    /// Creates a new terminal backend.
    /// </summary>
    /// <returns>A new backend.</returns>
    public static TerminalBackend CreateBackend() {
        if (false) {

        } else {
            return new ConsoleBackend();
        }
    }
    /// <summary>
    /// Creates a new terminal window (experimental).
    /// </summary>
    /// <returns>A new terminal window.</returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public static TerminalWindow CreateWindow() {
        
    }

    private static Thread? listenForKeysThread;
    private static bool listenForKeys = false;
    /// <summary>
    /// If it should listen for keys.
    /// </summary>
    public static bool ListenForKeys { set {
        if (value && (!listenForKeys)) {
            listenForKeys = value;
            listenForKeysThread = new Thread(ListenForKeysMethod);
            listenForKeysThread.Start();
        } else {
            listenForKeys = value;
        }
    } get {
        return listenForKeys;
    } }

    private static void ListenForKeysMethod() {
        while (listenForKeys) {
            ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control);
            OnKeyPress?.Invoke(key, keyChar, alt, shift, control);
        }
    }
    /// <summary>
    /// Reads a key from the terminal.
    /// </summary>
    /// <param name="key">The read key.</param>
    /// <param name="keyChar">The key character representing that key.</param>
    /// <param name="alt">True if the alt key was pressed.</param>
    /// <param name="shift">True if the shift key was pressed.</param>
    /// <param name="control">True if the control key was pressed.</param>
    public static void ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control) {
        backend.ReadKey(out key, out keyChar, out alt, out shift, out control);
    }
    /// <summary>
    /// If it should block CTRL + C.
    /// </summary>
    public static bool BlockCancelKey { get => backend.HideCursor; set => backend.HideCursor = value; }
    /// <summary>
    /// Waits until a key is pressed.
    /// </summary>
    public static void WaitForKeyPress() {
        ReadKey(out _, out _, out _, out _, out _);
    }
    /// <summary>
    /// An event for when a key is pressed.
    /// </summary>
    public static event KeyPressCallback? OnKeyPress;

    /// <summary>
    /// The out (to terminal) stream.
    /// </summary>
    public static TextWriter Out {get { return backend.StandardOutput; } }
    /// <summary>
    /// The in (from terminal) stream.
    /// </summary>
    public static TextReader In {get { return backend.StandardInput; } }
    /// <summary>
    /// The error (to terminal) stream.
    /// </summary>
    public static TextWriter Error {get { return backend.StandardError; } }
    /// <summary>
    /// Hides or shows terminal cursor.
    /// </summary>
    public static bool HideCursor { get => backend.HideCursor; set => backend.HideCursor = value; }
    /// <summary>
    /// The width (in characters) of the terminal.
    /// </summary>
    public static uint Width { get => backend.Size.Width; }
    /// <summary>
    /// The height (in characters) of the terminal.
    /// </summary>
    public static uint Height { get => backend.Size.Height; }
    /// <summary>
    /// The encoding used for the in stream (default: UTF-8).
    /// </summary>
    public static Encoding InputEncoding { get => backend.InputEncoding; set => backend.InputEncoding = value; }
    /// <summary>
    /// The encoding used for the error and out streams (default: UTF-8).
    /// </summary>
    public static Encoding OutputEncoding { get => backend.OutputEncoding; set => backend.OutputEncoding = value; }
    
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void Write(object? text, Style? style = null) {
        Out.Write((style ?? new Style()).ToANSI()+text?.ToString()+ANSI.Styles.ResetAll);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void WriteLine(object? text, Style? style = null) {
        Out.WriteLine((style ?? new Style()).ToANSI()+text?.ToString()+ANSI.Styles.ResetAll);
    }
    /// <summary>
    /// Writes a line to the terminal, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use.</param>
    public static void WriteLine(Style? style = null) {
        WriteLine(null, style);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <typeparam name="T">The type of what to write (<see cref="object.ToString"/>).</typeparam>
    /// <param name="text">The text to write to the error output stream.</param>
    /// <param name="style">The style to use (default: with red foreground).</param>
    public static void WriteErrorLine<T>(object? text, Style? style = null) {
        Error.WriteLine((style ?? new Style {ForegroundColor = Colors.Red}).ToANSI()+text?.ToString()+ANSI.Styles.ResetAll);
    }
    /// <summary>
    /// Writes a line to the error stream, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use (default: with red foreground).</param>
    public static void WriteErrorLine(Style? style = null) {
        WriteLine(null, style);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <param name="text">The text to write to the error output stream.</param>
    /// <param name="style">The style to use (default: with red foreground).</param>
    public static void WriteError(object? text, Style? style = null) {
        Error.Write((style ?? new Style {ForegroundColor = Colors.Red}).ToANSI()+text?.ToString()+ANSI.Styles.ResetAll);
    }
    /// <summary>
    /// Sets the cursor to that position.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void Goto((int x, int y) pos) {
        if (pos.x >= Width || pos.x < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos x is higher than the width or is lower than 0."); }
        if (pos.y >= Height || pos.y < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos y is higher than the height or is lower than 0."); }
        Out.Write(ANSI.MoveCursor(pos.x+1, pos.y+1));
    }
    /// <summary>
    /// Sets the cursor to that position in the error stream.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void GotoError((int x, int y) pos) {
        if (pos.x >= Width || pos.x < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos x is higher than the width or is lower than 0."); }
        if (pos.y >= Height || pos.y < 0) { throw new ArgumentOutOfRangeException(nameof(pos), "pos y is higher than the height or is lower than 0."); }
        Error.Write(ANSI.MoveCursor(pos.x+1, pos.y+1));
    }
    /// <summary>
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor position.</returns>
    public static (int x, int y) GetCursorPosition() {
        return backend.CursorPosition;
    }
    /// <summary>
    /// Sets the something (<see cref="object.ToString"/>) at a <paramref name="pos"/>, with a <paramref name="style"/>.
    /// </summary>
    /// <param name="text">The thing to set at <paramref name="pos"/> to the terminal.</param>
    /// <param name="pos">The position to set <paramref name="text"/> at.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void Set(object? text, (int x, int y) pos, Style? style = null) {
        Goto(pos);
        Write(text, style);
    }

    /// <summary>
    /// Sets the something in the error stream (<see cref="object.ToString"/>) at a <paramref name="pos"/>, with a <paramref name="style"/>.
    /// </summary>
    /// <param name="text">The thing to set at <paramref name="pos"/> to the terminal.</param>
    /// <param name="pos">The position to set <paramref name="text"/> at.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void SetError(object? text, (int x, int y) pos, Style? style = null) {
        Goto(pos);
        WriteError(text, style);
    }
    /// <summary>
    /// Reads one character from the input stream.
    /// </summary>
    /// <returns>The character that has been read (-1 if there is nothing to read).</returns>
    public static int Read() {
        return In.Read();
    }
    /// <summary>
    /// Reads a line from the input stream.
    /// </summary>
    /// <returns>The line that has been read (null if there is nothing to read).</returns>
    public static string? ReadLine() {
        return In.ReadLine();
    }

    /// <summary>
    /// Clears (resets) the whole screen.
    /// </summary>
    public static void Clear() {
        Goto((0,0));
        Out.Write(ANSI.EraseScreenFromCursor);
    }
    /// <summary>
    /// Clears screen from the position to end of the screen.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public static void ClearFrom((int x, int y) pos) {
        Goto(pos);
        Out.Write(ANSI.EraseScreenFromCursor);
    }
    /// <summary>
    /// Clears (deletes) a line.
    /// </summary>
    /// <param name="line">The y-axis of the line.</param>
    public static void ClearLine(int line) {
        Goto((0, line));
        Out.Write(ANSI.EraseLine);
    }
    /// <summary>
    /// Clears the line from the position to the end of the line.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public static void ClearLineFrom((int x, int y) pos) {
        Goto(pos);
        Out.Write(ANSI.EraseLineFromCursor);
    }
}
