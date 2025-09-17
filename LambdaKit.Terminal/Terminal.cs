using System.Runtime.Versioning;
using System.Text;
using LambdaKit.Terminal.Backend;
using LambdaKit.Terminal.Backend.Window;

namespace LambdaKit.Terminal;

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
    private static readonly TerminalBackend backend;
    static Terminal() {
        backend = CreateBackend();
        backend.OnKeyPress += 
            (key, keyChar, alt, shift, control) => OnKeyPress?.Invoke(key, keyChar, alt, shift, control);
        OutputEncoding = Encoding.UTF8;
        InputEncoding = Encoding.UTF8;
        BlockCancelKey = false;
    }

    #region Backend
    /// <summary>
    /// Creates a new terminal backend.
    /// </summary>
    /// <returns>A new backend.</returns>
    public static TerminalBackend CreateBackend() {
        return new ConsoleBackend();
    }
    /// <summary>
    /// Creates a new terminal window (experimental).
    /// </summary>
    /// <returns>A new terminal window.</returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    [SupportedOSPlatform("Windows")]
    public static TerminalWindow CreateWindow(string title = "Terminal") {
        #if OS_WINDOWS
        return new WindowsWindow(title);
        #else
        throw new PlatformNotSupportedException("Only Windows supports terminal windows.");
        #endif
    }
    #endregion

    #region Abstraction
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
    public static bool HideCursor { set => backend.HideCursor = value; }
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
    #endregion

    #region Writing
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void Write(object? text, Style? style = null, bool postReset = true) {
        backend.Write(text, style, postReset);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the terminal, with a style.
    /// </summary>
    /// <param name="text">The thing to write to the terminal.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteLine(object? text, Style? style = null, bool postReset = true) {
        backend.WriteLine(text, style, postReset);
    }
    /// <summary>
    /// Writes a line to the terminal, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use.</param>
    public static void WriteLine(Style? style = null) {
        backend.WriteLine(style);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <param name="text">The text to write to the error output stream.</param>
    /// <param name="style">The style to use (default: with red foreground).</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteErrorLine(object? text, Style? style = null, bool postReset = true) {
        backend.WriteErrorLine(text, style, postReset);
    }
    /// <summary>
    /// Writes a line to the error stream, with a style.
    /// </summary>
    /// <param name="style">The text decoration to use (default: with red foreground).</param>
    public static void WriteErrorLine(Style? style = null) {
        backend.WriteErrorLine(style);
    }
    /// <summary>
    /// Writes something (<see cref="object.ToString"/>) to the error stream, with a style.
    /// </summary>
    /// <param name="text">The text to write to the error output stream.</param>
    /// <param name="style">The style to use (default: with red foreground).</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteError(object? text, Style? style = null, bool postReset = true) {
        backend.WriteError(text, style, postReset);
    }
    /// <summary>
    /// Sets the cursor to that position.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void Goto((int x, int y) pos) {
        backend.Goto(pos);
    }
    /// <summary>
    /// Sets the cursor to that position in the error stream.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void GotoError((int x, int y) pos) {
        backend.GotoError(pos);
    }
    /// <summary>
    /// Sets the something (<see cref="object.ToString"/>) at a <paramref name="pos"/>, with a <paramref name="style"/>.
    /// </summary>
    /// <param name="text">The thing to set at <paramref name="pos"/> to the terminal.</param>
    /// <param name="pos">The position to set <paramref name="text"/> at.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void Set(object? text, (int x, int y) pos, Style? style = null) {
        backend.Goto(pos).Write(text, style);
    }

    /// <summary>
    /// Sets the something in the error stream (<see cref="object.ToString"/>) at a <paramref name="pos"/>, with a <paramref name="style"/>.
    /// </summary>
    /// <param name="text">The thing to set at <paramref name="pos"/> to the terminal.</param>
    /// <param name="pos">The position to set <paramref name="text"/> at.</param>
    /// <param name="style">The text decoration to use.</param>
    public static void SetError(object? text, (int x, int y) pos, Style? style = null) {
        backend.GotoError(pos).WriteError(text, style);
    }
    #endregion

    #region Reading
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

    #region Reading - Keys
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
    /// Waits until a key is pressed.
    /// </summary>
    public static void WaitForKeyPress() {
        backend.WaitForKeyPress();
    }
    /// <summary>
    /// An event for when a key is pressed.
    /// </summary>
    public static event KeyPressCallback? OnKeyPress;

    /// <summary>
    /// If it should listen for keys.
    /// </summary>
    public static bool ListenForKeys { set {
        backend.ListenForKeys = value;
    } get {
        return backend.ListenForKeys;
    } }
    /// <summary>
    /// If it should block CTRL + C.
    /// </summary>
    public static bool BlockCancelKey { get => backend.BlockCancelKey; set => backend.BlockCancelKey = value; }
    #endregion

    #endregion

    #region Screen
    /// <summary>
    /// Saves the current screen and loads another (XTerm) (opposite of <see cref="UseNormalScreen"/>).
    /// </summary>
    public static void UseAlternativeScreen() {
        backend.UseAlternativeScreen();
    }
    /// <summary>
    /// Loads the old screen (XTerm) (opposite of <see cref="UseAlternativeScreen"/>).
    /// </summary>
    public static void UseNormalScreen() {
        backend.UseNormalScreen();
    }
    /// <summary>
    /// Saves the current screen and loads another, also saving the cursor position (XTerm) (opposite of <see cref="UseNormScreenAndRestoreCursor"/>).
    /// </summary>
    public static void UseAltScreenAndSaveCursor() {
        backend.UseAltScreenAndSaveCursor();
    }
    /// <summary>
    /// Loads the old screen, also restoring the cursor position (XTerm) (opposite of <see cref="UseNormScreenAndRestoreCursor"/>).
    /// </summary>
    public static void UseNormScreenAndRestoreCursor() {
        backend.UseNormScreenAndRestoreCursor();
    }
    /// <summary>
    /// Scrolls up. Adds new lines at the bottom.
    /// </summary>
    /// <param name="amount">How many lines to scroll up.</param>
    public static void ScrollUp(uint amount = 1) {
        backend.ScrollUp(amount);
    }
    /// <summary>
    /// Scrolls down. Adds new lines at the top.
    /// </summary>
    /// <param name="amount">How many lines to scroll down.</param>
    public static void ScrollDown(uint amount = 1) {
        backend.ScrollDown(amount);
    }
    #endregion

    #region Cursor
    /// <summary>
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor position.</returns>
    public static (int x, int y) GetCursorPosition() {
        return backend.GetCursorPosition();
    }
    /// <summary>
    /// Saves the cursor position (DEC).
    /// </summary>
    public static void SaveCursorPosition() {
        backend.SaveCursorPosition();
    }
    /// <summary>
    /// Restores the cursor position (DEC).
    /// </summary>
    public static void RestoreCursorPosition() {
        backend.RestoreCursorPosition();
    }
    #endregion

    #region Clearing
    /// <summary>
    /// Clears everything, including scrollback buffer (XTerm). 
    /// </summary>
    public static void ClearAll() {
        backend.ClearAll();
    }
    /// <summary>
    /// Clears (resets) the whole screen.
    /// </summary>
    public static void ClearScreen() {
        backend.ClearScreen();
    }
    /// <summary>
    /// Clears screen from the position to end of the screen. Moves cursor to that position.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public static void ClearFrom((int x, int y) pos) {
        backend.ClearFrom(pos);
    }
    /// <summary>
    /// Clears a line. Moves cursor to that line.
    /// </summary>
    /// <param name="line">The y-axis of the line.</param>
    public static void ClearLine(int line) {
        backend.ClearLine(line);
    }
    /// <summary>
    /// Clears the line from the position to the end of the line. Moves cursor to that position.
    /// </summary>
    /// <param name="pos">The start position.</param>
    public static void ClearLineFrom((int x, int y) pos) {
        backend.ClearLineFrom(pos);
    }
    #endregion
    
    #region Misc
    /// <summary>
    /// Makes an audible noise.
    /// </summary>
    public static void Bell() {
        backend.Bell();
    }
    /// <summary>
    /// Writes a link to a URI in the terminal (OSC 8).
    /// </summary>
    /// <param name="uri">The URI to write.</param>
    /// <param name="label">The label to give.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteHyperlink(Uri uri, string label, Style? style = null, bool postReset = true) {
        backend.WriteHyperlink(uri, label, style, postReset);
    }
    /// <summary>
    /// Writes a link to a URI in the terminal (OSC 8).
    /// </summary>
    /// <param name="uri">The URI to write.</param>
    /// <param name="label">The label to give.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteHyperlinkLine(Uri uri, string label, Style? style = null, bool postReset = true) {
        backend.WriteHyperlink(uri, label, style, postReset);
    }
    /// <summary>
    /// Writes a link to a URI in the terminal (OSC 8).
    /// </summary>
    /// <param name="uri">The URI to write.</param>
    /// <param name="label">The label to give.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteHyperlinkError(Uri uri, string label, Style? style = null, bool postReset = true) {
        backend.WriteHyperlink(uri, label, style, postReset);
    }
    /// <summary>
    /// Writes a link to a URI in the terminal (OSC 8).
    /// </summary>
    /// <param name="uri">The URI to write.</param>
    /// <param name="label">The label to give.</param>
    /// <param name="style">The text decoration to use.</param>
    /// <param name="postReset">If it should reset all the styles afterwards.</param>
    public static void WriteHyperlinkErrorLine(Uri uri, string label, Style? style = null, bool postReset = true) {
        backend.WriteHyperlink(uri, label, style, postReset);
    }
    #endregion
}
