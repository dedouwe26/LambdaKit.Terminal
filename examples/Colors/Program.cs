// The namespace of the terminal.
using OxDED.Terminal;

class Program {
    public static void Main(string[] args) {
        // * Using style.
        Terminal.WriteLine("Red\n", new Style{ForegroundColor = RGBColor.Red});

        // Printing out all the terminal-defined colors
        foreach (StandardColor.Colors color in Enum.GetValues<StandardColor.Colors>()) {
            Terminal.Write(color.ToString()+' ', new Style() { ForegroundColor = (StandardColor)color});
        }

        Terminal.WriteLine("\n");

        // Random byte for pallete color and font.
        byte[] randomBytes = new byte[1];
        Random.Shared.NextBytes(randomBytes);

        // Using Style builder for multiple styles in one go.
        Terminal.WriteLine(
            new StyleBuilder()
            .Foreground(RGBColor.Green).Text("Green\t\t").ResetForeground()        .Blink().Text("Blinking").Blink(false).NewLine()
            .Foreground(RGBColor.Blue).Text("Blue\t\t").ResetForeground()          .Bold().Text("Bold").Bold(false).NewLine()
            .Foreground(RGBColor.LightRed).Text("LightRed\t").ResetForeground()    .Faint().Text("Faint").Faint(false).NewLine()
            .Foreground(RGBColor.DarkGreen).Text("DarkGreen\t").ResetForeground()  .Underline().Text("Underline").Underline(false).NewLine()
            .Foreground(RGBColor.DarkBlue).Text("DarkBlue\t").ResetForeground()    .DoubleUnderline().Text("Double underline").DoubleUnderline(false).NewLine()
            .Foreground(RGBColor.Yellow).Text("Yellow\t\t").ResetForeground()      .Striketrough().Text("Striketrough").Striketrough(false).NewLine()
            .Foreground(RGBColor.Orange).Text("Orange\t\t").ResetForeground()      .Inverse().Text("Inverse").Inverse(false).NewLine()
            .Foreground(RGBColor.White).Text("White\t\t").ResetForeground()        .Invisible().Text("Invisible, you dont see me!").Invisible(false).NewLine()
            .Foreground(RGBColor.Gray).Text("Gray\t\t").ResetForeground()          .Italic().Text("Italic").Italic(false).NewLine()
            .Foreground(RGBColor.Black).Text("Black\t\t").ResetForeground()        .Overline().Text("Line above").Overline(false).NewLine()
            .Foreground(new PalleteColor(randomBytes[0])).Text("Pallete\t\t")      .Font(9).Text("Font").Font(null)
        );

        // Tests Interference with other styles.
        Terminal.WriteLine("Test for bold interference with faint", new Style{Bold = true});
        Terminal.WriteLine("Test for underline interference with double underline", new Style{Underline = true});

        // Simple key listener (see: Keypresses example).
        Terminal.ListenForKeys = true;
        Terminal.WriteLine("Press X to clear terminal...");
        Terminal.OnKeyPress += (key, keyChar, alt, shift, control) => {
            // If X is pressed, clear terminal.
            if (key == ConsoleKey.X) {
                Terminal.ClearAll();
            }
            // Stops program.
            Terminal.ListenForKeys = false;
        };
    }
}