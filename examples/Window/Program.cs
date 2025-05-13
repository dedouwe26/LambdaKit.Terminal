// The namespace of the terminal and terminal windowing.
using System.Runtime.Versioning;
using LambdaKit.Terminal;
using LambdaKit.Terminal.Backend.Window;

class Program {
    [SupportedOSPlatform("Windows")]
    public static void Main(string[] args) {
        TerminalWindow window = Terminal.CreateWindow("test");
        window.WriteLine("lesgo");
        window.ReadKey(out _, out char keyC, out _, out _, out _);
        window.Dispose();
        Terminal.WaitForKeyPress();
        Terminal.WriteLine(keyC);
    }
}