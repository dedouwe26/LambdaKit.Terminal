namespace LambdaKit.Terminal.Arguments;

/// <summary>
/// An exception that occures when the parser catches an error.
/// </summary>
[Serializable]
public class ArgumentParserException : Exception {
    /// <summary>
    /// Creates a new argument parser exception.
    /// </summary>
    public ArgumentParserException() { }
    /// <summary>
    /// Creates a new argument parser exception with message.
    /// </summary>
    /// <param name="message">The message to give.</param>
    public ArgumentParserException(string message) : base(message) { }
}