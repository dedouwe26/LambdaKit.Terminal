using System.Text;

namespace OxDED.Terminal.Backend;

/// <summary>
/// The terminal back-end implementation made by .NET.
/// </summary>
public class ConsoleBackend : TerminalBackend {
    /// <inheritdoc/>
    public override TextReader StandardInput => Console.In;
    /// <inheritdoc/>
    public override TextWriter StandardOutput => Console.Out;
    /// <inheritdoc/>
    public override TextWriter StandardError => Console.Error;
    /// <inheritdoc/>

    /// <inheritdoc/>
    public override Encoding InputEncoding { get => Console.InputEncoding; set => Console.InputEncoding = value; }
    /// <inheritdoc/>
    public override Encoding OutputEncoding { get => Console.OutputEncoding; set => Console.OutputEncoding = value; }

    /// <inheritdoc/>
    public override (int x, int y) GetCursorPosition() {
        return Console.GetCursorPosition();
    }

    /// <inheritdoc/>
    public override void WaitForKeyPress() {
        Console.ReadKey(true);
    }

    /// <inheritdoc/>
    protected override void ListenForKeysMethod() {
        while (listenForKeys) {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (!Console.IsInputRedirected) {
                KeyPress(key.Key, key.KeyChar, key.Modifiers.HasFlag(ConsoleModifiers.Alt), key.Modifiers.HasFlag(ConsoleModifiers.Shift), key.Modifiers.HasFlag(ConsoleModifiers.Control));
            }
        }
    }
    /// <inheritdoc/>
    public override void Dispose() {
        GC.SuppressFinalize(this);
    }
}