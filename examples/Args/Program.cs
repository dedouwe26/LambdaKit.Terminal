using LambdaKit.Terminal;
using LambdaKit.Terminal.Arguments;

class Program {
    public static void Main(string[] args) {
        ArgumentParser parser = new ArgumentFormatter()
            .Name("examples - Argument parsing") // NOTE: The name, version and description are used by 
            .Version("1.0.0")                    //       the help and version commands. 
            .Description("Shows how argument parsing works.") // These does support styling.

            //   Arguments | Options
            // - required  | Not required
            // - positional| Not positional
            // - No keys   | Uses keys

            .General() // Creates a general category.
                .Argument() // Starts a new argument.
                    .Name("first") // The name of the argument. NOTE: This is used in the help menu.
                    .Description("This is the first argument.")
                .Finish() // Stops and saves the new argument.
            .Finish() // Stops and saves the general category.
            
            .Category() // Creates a normal category with a name.
                .Name("Another category") // The name of the category.
                .Option() // Starts a new option.
                    .Keys(["a", "first"]) // The keys used to declare this option.
                    .Description("This is the first option.") // The description of the first option.
                .Finish() // Stops and saves the new option.
                .Option() // Creates another option.
                    .Key("b").Key("second") 
                    .Description("This is the second option.")
                .Finish()
            .Finish() // Stops and saves the category.

            .Category() // Creates an unnamed category
                .Option() // Creates the third option.
                    .Key("c")
                    .Description("First option with parameters.")
                    .Parameter() // Creates a new required parameter.
                        .Name("first") // The name of this parameter.
                        .Description("First parameter.") // The description of this parameter.
                    .Finish()
                .Finish()
            .Finish()

            .General() // The same general category as at the beginning.
                .HelpOption() // Creates a help option.
                    // .Key("d") // The keys default to: ["h", "help"]. NOTE: The description has a default too.
                    // .Quit(false) // The program quits by default when the help option is used.
                .Finish()
                .VersionOption() // Creates a version option.
                    // .Key("d") // The keys default to: ["v", "version"]. NOTE: The description has a default too.
                    // .Quit(false) // The program quits by default when the help option is used.
                .Finish()
            .Finish()
        .Finish();

        parser.Parse(args);

        // Commands to try out:
        // - Args.exe "Hello, world!" --first --bc Required
        // - Args.exe "argument" -ab -c "parameter"     NOTE: "-ab" is the same thing as: "-a -b".
        // - Args.exe -h
        // - Args.exe --version

        Terminal.WriteLine(parser.GetArgument(0)!.Content); // NOTE: Can use !, because there is an argument.
        Terminal.WriteLine("Is '--first' used        : "+parser.HasKeyBeenUsed("first"));
        Terminal.WriteLine("Is the second option used: "+parser.HasOption("second"));
        string? parameter = parser.GetOption("c")?.Parameters?[0];
        Terminal.WriteLine("Third option's parameter : "+(parameter ?? "(not used)").ToString());
    }
}