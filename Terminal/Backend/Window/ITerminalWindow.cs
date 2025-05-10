namespace OxDED.Terminal.Backend.Window;

/// <summary>
/// Represents a terminal window that can be used.
/// </summary>
public interface ITerminalWindow : ITerminalBackend {
    /// <summary>
    /// Gets or sets the title of the terminal window.
    /// </summary>
    public string Title { get; set; }
}