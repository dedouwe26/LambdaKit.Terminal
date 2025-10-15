using LambdaKit.Terminal;
using static LambdaKit.Terminal.StandardColor;

class Program {
    public static void Main() {
        // NOTE: This is being written to the normal screen.
        Terminal.WriteLine("Loading alternate screen...", new Style() {ForegroundColor = (StandardColor)Colors.Green});

        /// Loads the alternative screen and saves the cursor position.
        Terminal.UseAltScreenAndSaveCursor();

        // Clears everything the scrollback part.
        Terminal.ClearAll(); // Everything should already be cleared, but just in case.
        Terminal.ClearScreen(); // NOTE: If a terminal does not support the above.
        Terminal.Goto((0, 0)); // Moves the cursor back to the beginning.

        Terminal.WriteLine("This should be an alternate buffer!", new Style() {ForegroundColor = (StandardColor)Colors.Red});
        Terminal.WriteLine("Press ANY KEY to return...", new Style() {ForegroundColor = (StandardColor)Colors.Red, Underline = true});

        Terminal.WaitForKeyPress();

        // Loads back the old screen and restores the cursor's position.
        Terminal.UseNormScreenAndRestoreCursor();
    }
}