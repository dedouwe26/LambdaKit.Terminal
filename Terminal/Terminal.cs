using System.Runtime.InteropServices;
using System.Text;
using OxDED.Terminal.Backend;

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
        OutEncoding = Encoding.UTF8;
        InEncoding = Encoding.UTF8;
        BlockCancelKey = false;
        Console.CancelKeyPress+=(sender, e)=>{
            e.Cancel = BlockCancelKey;
        };
    }
    private static Thread? listenForKeysThread;
    private static bool listenForKeys = false;
    /// <summary>
    /// If it should listen for keys.
    /// </summary>
    public static bool ListenForKeys {set {
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
    /// If it should block CTRL + C.
    /// </summary>
    public static bool BlockCancelKey { get; set; }
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
    public static uint Width {get { return (uint)Console.WindowWidth; }}
    /// <summary>
    /// The height (in characters) of the terminal.
    /// </summary>
    public static uint Height {get { return (uint)Console.WindowHeight; }}
    /// <summary>
    /// The encoding used for the in stream (default: UTF-8).
    /// </summary>
    public static Encoding InEncoding {get { return Console.InputEncoding; } set {Console.InputEncoding = value; }}
    /// <summary>
    /// The encoding used for the error and out streams (default: UTF-8).
    /// </summary>
    public static Encoding OutEncoding {get { return Console.OutputEncoding; } set {Console.OutputEncoding = value; }}
    /// <summary>
    /// Creates a new Terminal Window (Experimental).
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public static TerminalBackend CreateBackend() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            return new WinTerminalWindow(title);
        } else {
            // NOTE: Default to Dotnet implementation.
            return new ConsoleBackend();
        }
    }
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
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor position.</returns>
    public static (int x, int y) GetCursorPosition() {
        return Console.GetCursorPosition();
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
    /// Waits until a key is pressed.
    /// </summary>
    public static void WaitForKeyPress() {
        Console.ReadKey(true);
    }
    /// <summary>
    /// An event for when a key is pressed.
    /// </summary>
    public static event KeyPressCallback? OnKeyPress;

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
    private static void ListenForKeysMethod() {
        while (listenForKeys) {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (!Console.IsInputRedirected) {
                OnKeyPress?.Invoke(key.Key, key.KeyChar, key.Modifiers.HasFlag(ConsoleModifiers.Alt), key.Modifiers.HasFlag(ConsoleModifiers.Shift), key.Modifiers.HasFlag(ConsoleModifiers.Control));
            }
        }
    }
}
