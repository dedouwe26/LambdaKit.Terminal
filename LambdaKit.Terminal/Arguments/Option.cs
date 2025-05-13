using static LambdaKit.Terminal.Arguments.ArgumentFormatter;

namespace LambdaKit.Terminal.Arguments;

/// <summary>
/// Represents a parsed option.
/// </summary>
public class Option {
    /// <summary>
    /// The format of this option.
    /// </summary>
    public readonly OptionFormat Format;
    /// <summary>
    /// The key used to declare this option.
    /// </summary>
    public readonly string Key;
    /// <summary>
    /// The parameters of this option, null if this option cannot have parameters.
    /// </summary>
    public readonly string[]? Parameters;

    internal Option(OptionFormat format, string key, string[]? parameters) {
        Format = format;
        Key = key;
        Parameters = parameters;
    }
}