namespace LambdaKit.Terminal.Logging;

/// <summary>
/// Logger severity (from high severity to low severity).
/// </summary>
public enum Severity : byte {
    /// 
    Fatal,
    /// 
    Error,
    /// 
    Warning,
    /// 
    Message,
    /// 
    Info,
    /// 
    Debug,
    /// 
    Trace
}