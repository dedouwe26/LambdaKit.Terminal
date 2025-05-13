// The namespace of terminal logging.
using LambdaKit.Terminal;
using LambdaKit.Terminal.Logging;
using LambdaKit.Terminal.Logging.Targets;

class Program {
    public const string LoggerID = "me.0xDED.Terminal.examples";

    // Creates a logger with an ID (optional) and name and with severity Trace (lowest).
    public static Logger logger = new("Logging Example", LoggerID, Severity.Trace);
    public static void Main() {
        logger.LogMessage("Hello, world!");
        logger.LogInfo("This is the start of the program!");
        logger.LogWarning("This is a warning.");
        logger.LogDebug("Let's debug that.");
        logger.LogTrace("It came from there.");

        // Gets logger with logger id.
        Loggers.Get(LoggerID)?.LogError("It happend again.");
        Loggers.Get(LoggerID)?.LogFatal("Bye");

        Terminal.WriteLine();
        Terminal.WriteLine("Sub loggers");

        // Sub loggers
        SubLogger sublogger1 = logger.CreateSubLogger("sub1", "Sub 1", severity:Severity.Trace);
        sublogger1.LogInfo("This is the first sub logger of "+logger.Name);

        SubLogger sublogger2 = logger.CreateSubLogger("sub2", "Sub 2", severity:Severity.Trace);
        sublogger2.LogMessage("This is the second sub logger of "+sublogger2.ParentLogger.Name); // Gets parent name from ParentLogger

        SubLogger subsublogger = sublogger2.CreateSubLogger("sub", "sub-sub", severity:Severity.Trace);

        // sublogger2.SubLoggers[sublogger2.SubLoggers.Keys.ToArray()[0]]
        // NOTE: The difference between child ID and ID is that the child ID is used by the parent (last bit) and
        //       the ID is used by the Loggers Register (full ID).
        //  - child ID : "sub-sub"
        //  -       ID : "Logging Example.Sub 2.sub-sub"
        sublogger2.GetSubLogger(subsublogger.childID)!.LogTrace("This is the sub logger of "+sublogger2.Name); // Gets sublogger from parent

        // Tree of loggers (names of variables):
        // logger + sublogger1
        //        |
        //        + sublogger2 - subsublogger

        Terminal.WriteLine();

        // Change name format of terminal target, can also be done with FileTarget:
        subsublogger.GetTarget<TerminalTarget>()!.NameFormat = "{0} - {1}"; 
        subsublogger.LogDebug("<<< Different name format");

        // Change message format, can also be done with FileTarget:
        sublogger2.GetTarget<TerminalTarget>(0)!.Format = "<{1}>: {3}: ({2}) : {5}{4}"+ANSI.Styles.ResetAll;
        // NOTE: Knowing the index is faster with more targets!
        
        sublogger2.LogDebug("Wow cool new format!");

        // Can listen for unhandled exceptions in the current app domain.
        logger.HandleUnhandledExceptions = true;

        // try {
        //     throw new Exception("middle", new Exception("inner"));
        // } catch (Exception e) {
        //     throw new Exception("outer", e);
        // }

        // Can also log exceptions.
        logger.LogException(new Exception("outer", new Exception("middle", new Exception("inner"))));


        // Don't forget to dispose the logger.
        logger.Dispose();
    }
}