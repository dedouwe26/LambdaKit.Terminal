using System.Text;

namespace LambdaKit.Terminal.Backend;

/// <summary>
/// Represents a terminal backend implementation by the CLR.
/// </summary>
public class ConsoleBackend : TerminalBackend {
    /// <summary>
    /// Creates a new dotnet terminal backend.
    /// </summary>
    public ConsoleBackend() {
        HideCursor = false;
        Console.CancelKeyPress += (sender, e) => {
            if (BlockCancelKey) {
                e.Cancel = true;
            }
        };
    }
    /// <inheritdoc/>
    public override TextReader StandardInput => Console.In;
    /// <inheritdoc/>
    public override TextWriter StandardOutput => Console.Out;
    /// <inheritdoc/>
    public override TextWriter StandardError => Console.Error;

    /// <inheritdoc/>
    public override Encoding InputEncoding { get => Console.InputEncoding; set => Console.InputEncoding = value; }
    /// <inheritdoc/>
    public override Encoding OutputEncoding { get => Console.OutputEncoding; set => Console.OutputEncoding = value; }
    /// <inheritdoc/>
    public override Encoding ErrorEncoding { get => Console.OutputEncoding; set => Console.OutputEncoding = value; }

    /// <inheritdoc/>
    public override (uint Width, uint Height) Size { get => ((uint)Console.WindowWidth, (uint)Console.WindowHeight); set {
        Console.WindowWidth = (int)value.Width;
        Console.WindowHeight = (int)value.Height;
    } }

    private bool cursorHidden = false;
    /// <inheritdoc/>
    public override bool HideCursor { get => cursorHidden; set { Console.CursorVisible = !value; cursorHidden = value; } }
    /// <inheritdoc/>
    public override (int x, int y) CursorPosition { get => Console.GetCursorPosition(); set => Console.SetCursorPosition(value.x, value.y); }
    /// <inheritdoc/>
    public override bool BlockCancelKey { get; set; }
    

    /// <inheritdoc/>
    public override void Dispose() { GC.SuppressFinalize(this); }

    /// <inheritdoc/>
    public override bool ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control) {
        ConsoleKeyInfo keyInfo;
        try {
            keyInfo = Console.ReadKey(true);
        } catch (InvalidOperationException) {
            key = default;
            keyChar = default;
            alt = default;
            shift = default;
            control = default;
            return false;
        }
        
        key = keyInfo.Key;
        keyChar = keyInfo.KeyChar;
        alt = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt);
        shift = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift);
        control = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
        return true;
    }
    /// <inheritdoc/>
    public override void WaitForKeyPress() {
        Console.ReadKey(true);
    }
    /// <inheritdoc/>
    public override (int x, int y) GetCursorPosition() {
        return Console.GetCursorPosition();
    }
}