// The namespace of the terminal.
using LambdaKit.Terminal;

class Program {
    public static sbyte countdown = 5;
    public static void Main() {
		Terminal.HideCursor = true;
		// Will listen for keys (non-blocking).
		Terminal.ListenForKeys = true;
        // Adds an event.
        Terminal.OnKeyPress += OnKey;
	}
    public static void OnKey(ConsoleKey key, char keyChar, bool alt, bool shift, bool control) {
        // Says what key has been pressed, and shows the countdown
        Terminal.WriteLine(key.ToString() + " " + countdown.ToString(), new Style() { Bold = control, Italic = alt, Underline = shift});

        // lowers the countdown and checks if it is at zero.
        if ((--countdown)==0) {
            // Stops listening for keys.
            Terminal.ListenForKeys = false;

            Terminal.WriteLine("Waiting for input...", new Style{ Bold = true });
            //Waits for key press.
            Terminal.WaitForKeyPress();
            // End of program
		    Terminal.HideCursor = false;
        }
    }
}