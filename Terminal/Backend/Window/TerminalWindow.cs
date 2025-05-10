namespace OxDED.Terminal.Backend.Window;

/// <summary>
/// Represents a terminal window that can be used.
/// </summary>
public abstract class TerminalWindow : TerminalBackend, ITerminalWindow {
    /// <summary>
    /// The title of the Terminal window.
    /// </summary>
    public abstract string Title { get; set; }
}