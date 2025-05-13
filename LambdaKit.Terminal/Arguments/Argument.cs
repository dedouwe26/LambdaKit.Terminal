using static LambdaKit.Terminal.Arguments.ArgumentFormatter;

namespace LambdaKit.Terminal.Arguments;

/// <summary>
/// Represents a parsed option.
/// </summary>
public class Argument {
    /// <summary>
    /// The format of this argument.
    /// </summary>
    public readonly ArgumentFormat Format;
    /// <summary>
    /// The content of this argument.
    /// </summary>
    public readonly string Content;

    internal Argument(ArgumentFormat format, string content) {
        Format = format;
        Content = content;
    }
}