namespace LambdaKit.Terminal.Arguments;

/// <summary>
/// The final stage of argument parsing.
/// </summary>
public class ArgumentParser {
    /// <summary>
    /// A parser that parses the arguments.
    /// </summary>
    public class Parser {
        /// <summary>
        /// The identifier that identifies the beginning of an option.
        /// </summary>
        public const char Identifier = '-';
        private readonly ArgumentFormatter format;
        /// <summary>
        /// Creates a new argument parser.
        /// </summary>
        /// <param name="arguments">The arguments used for parsing.</param>
        /// <param name="format">The format of the arguments.</param>
        public Parser(string[] arguments, ArgumentFormatter format) {
            this.arguments = arguments;
            this.format = format;
        }

        private int argumentIndex = 0;
        private int ArgumentIndex {
            get {
                return argumentIndex;
            }
            set {
                if (argumentIndex != value) {
                    argumentIndex = value;
                    stream = null;
                }
            }
        }
        private StringReader? stream;
        private StringReader Stream {
            get {
                return stream ??= new StringReader(arguments[ArgumentIndex]);
            }
        }
        private readonly string[] arguments;

        /// <summary>
        /// The parsed options.
        /// </summary>
        public readonly List<Option> Options = [];
        /// <summary>
        /// The parsed arguments.
        /// </summary>
        public readonly List<Argument> Arguments = [];

        /// <summary>
        /// Starts parsing the 
        /// </summary>
        /// <exception cref="ArgumentParserException"></exception>
        public void Parse() {
            while (HasNext()) {
                char peeked = Peek(); // The should be a next available.
                if (peeked == Identifier) {
                    ReadOption();
                } else {
                    ReadArgument();
                }
            }
        }

        private char Read(string failMsg = "Could not read next character.") {
            int read;
            try {
                read = Stream.Read();
            } catch (IndexOutOfRangeException) {
                throw new ArgumentParserException(failMsg);
            }

            if (read == -1) { ArgumentIndex++; return Read(failMsg); }
            return (char)read;
        }
        private char Peek(string failMsg = "Could not read next character.") {
            int peeked;
            try {
                peeked = Stream.Peek();
            } catch (IndexOutOfRangeException) {
                throw new ArgumentParserException(failMsg);
            }

            if (peeked == -1) { ArgumentIndex++; return Peek(failMsg); }
            return (char)peeked;
        }
        private bool HasNext() {
            int peeked;
            try {
                peeked = Stream.Peek();
            } catch (IndexOutOfRangeException) {
                return false;
            }
            
            if (peeked == -1) { ArgumentIndex++; return HasNext(); }
            return true;
        }
        private bool HasNextInPart() {
            int peeked;
            try {
                peeked = Stream.Peek();
            } catch (IndexOutOfRangeException) {
                return false;
            }
            if (peeked == -1) { return false; }
            return true;
        }

        private string ReadPart(string failMsg = "Not enough parameters given.") {
            string part;
            try {
                part = Stream.ReadToEnd();
            } catch (IndexOutOfRangeException) {
                throw new ArgumentParserException(failMsg);
            }
            if (part == "") {
                ArgumentIndex++;
                return ReadPart(failMsg);
            }
            ArgumentIndex++;
            return part;
        }

        private void ReadOption() {
            Read();

            if (Peek("Could not read next option.") == Identifier) {
                Read();
                // Read long option (--... ???)
                string key = ReadPart($"No option name found after '{Identifier}{Identifier}'.");

                OptionFormat format = this.format.AllOptions.FirstOrDefault((OptionFormat f) => f.keys.Contains(key))
                    ?? throw new ArgumentParserException($"No option found with name: '{key}'.");
                
                if (format.parameters.Count > 0) {
                    ReadOptionParameters(format, key);
                } else {
                    Options.Add(new Option(format, key, null));
                }
            } else {
                // Read short option (-.??? ???)
                do {
                    char key = Read($"No option name found after '{Identifier}'.");
                    OptionFormat format = this.format.AllOptions.FirstOrDefault((OptionFormat f) => f.keys.Contains(key.ToString()))
                        ?? throw new ArgumentParserException($"No option found with name: '{key}'.");
                    
                    if (format.parameters.Count > 0) {
                        ReadOptionParameters(format, key.ToString());
                        break;
                    }
                    Options.Add(new Option(format, key.ToString(), null));
                } while (HasNextInPart());
            }
        }
        private void ReadOptionParameters(OptionFormat format, string usedKey) {
            List<string> parameters = [];
            while (parameters.Count < format.parameters.Count) {
                string read = ReadPart($"Not enough option parameters for '{usedKey}'. Requires {format.parameters.Count}, only got {parameters.Count}");

                parameters.Add(read);
            }
            Options.Add(new Option(format, usedKey, [.. parameters]));
        }
        private void ReadArgument() {
            if (Arguments.Count >= format.AllArguments.Count) throw new ArgumentParserException($"Too many arguments. Requires {format.AllArguments.Count}, got {Arguments.Count+1}.");
            string content = ReadPart($"Not enough arguments. Requires {format.AllArguments.Count}, only got {Arguments.Count}.");

            Arguments.Add(new Argument(format.AllArguments[Arguments.Count], content));
        }
    }
    
    /// <summary>
    /// The format used for parsing.
    /// </summary>
    public readonly ArgumentFormatter Format;

    /// <summary>
    /// The parsed options.
    /// </summary>
    public Option[] Options { get; private set; }
    /// <summary>
    /// The parsed arguments.
    /// </summary>
    public Argument[] Arguments { get; private set; }
    /// <summary>
    /// Creates a new argument parser.
    /// </summary>
    /// <param name="format"></param>
    public ArgumentParser(ArgumentFormatter format) {
        Format = format;
        Options = [];
        Arguments = [];
    }
    /// <summary>
    /// Parses the arguments.
    /// </summary>
    /// <param name="args">The arguments to parse.</param>
    public void Parse(string[] args) {
        Parser parser = new(args, Format);
        try {
            parser.Parse();
        } catch (ArgumentParserException e) {
            ShowError(e.Message);
            if (Format.shouldExitOnError) Environment.Exit(1);
        }

        Options = [.. parser.Options];
        Arguments = [.. parser.Arguments];

        if (Format.CurrentHelpOption != null && HasOption(Format.CurrentHelpOption)) {
            ShowHelp();
            if (Format.CurrentHelpOption.quit) Environment.Exit(0);
        }
        if (Format.CurrentVersionOption != null && HasOption(Format.CurrentVersionOption)) {
            ShowVersion();
            if (Format.CurrentVersionOption.quit) Environment.Exit(0);
        }

        if (Arguments.Length < Format.AllArguments.Count) {
            ShowError($"Too few arguments. Requires {Format.AllArguments.Count}, only got {Arguments.Length}.");
            if (Format.shouldExitOnError) Environment.Exit(1);
        }
    }

    /// <summary>
    /// Checks if the corresponding option is used.
    /// </summary>
    /// <param name="key">The key of the option of which the usage will be checked.</param>
    /// <returns>True the corresponding option is used.</returns>
    public bool HasOption(string key) {
        foreach (Option option in Options) {
            if (option.Format.keys.Contains(key)) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Checks if this key has been used.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key has been found.</returns>
    public bool HasKeyBeenUsed(string key) {
        foreach (Option option in Options) {
            if (option.Key == key) {
                return true;
            }
        }
        return false;
    } 
    /// <summary>
    /// Checks if the corresponding option is used.
    /// </summary>
    /// <param name="format">The format of the option of which the usage will be checked.</param>
    /// <returns></returns>
    public bool HasOption(OptionFormat format) {
        foreach (Option option in Options) {
            if (format.keys.Contains(option.Key)) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Gets the option from that key (like <see cref="HasOption(string)"/>).
    /// </summary>
    /// <param name="key">The key of the option that it will search for.</param>
    /// <returns>The option, null if it was not used.</returns>
    public Option? GetOption(string key) {
        foreach (Option option in Options) {
            if (option.Format.keys.Contains(key)) {
                return option;
            }
        }
        return null;
    }
    /// <summary>
    /// Gets the option from that format (like <see cref="HasOption(OptionFormat)"/>).
    /// </summary>
    /// <param name="format">The format of the option that it will search for.</param>
    /// <returns>The option, null if it was not used.</returns>
    public Option? GetOption(OptionFormat format) {
        foreach (Option option in Options) {
            if (format.keys.Contains(option.Key)) {
                return option;
            }
        }
        return null;
    }
    /// <summary>
    /// Gets the argument at that index.
    /// </summary>
    /// <param name="index">The index of that argument.</param>
    /// <returns>The argument, null if <paramref name="index"/> is out of range.</returns>
    public Argument? GetArgument(int index) {
        if (index >= Arguments.Length) return null;
        if (index < 0) return null;
        return Arguments[index];
    }

    /// <summary>
    /// Shows an argument parsing error in the terminal with help (see: <see cref="ShowHelp"/>).
    /// </summary>
    /// <param name="message">The error message that occured while argument parsing.</param>
    public void ShowError(string message) {
        Terminal.WriteLine(message);
        ShowHelp(false, false);
    }

    /// <summary>
    /// Shows the options and arguments to the screen.
    /// </summary>
    /// <param name="showName">If the name and version should be shown.</param>
    /// <param name="showDescription">If the description of the program should be shown.</param>
    public void ShowHelp(bool showName = true, bool showDescription = true) {
        // Name and description.
        ShowVersion(showName, showDescription);

        string msg = "\n";

        foreach (CategoryFormat category in Format.Categories) {
            msg+=new StyleBuilder().Bold().Text(category.name+":").Bold(false).NewLine();

            foreach (ArgumentFormat argument in category.Arguments) {
                msg+="\t"+new StyleBuilder().Foreground((StandardColor)StandardColor.Colors.Cyan).Text(argument.name).ResetForeground().Text(": "+argument.description).NewLine().ToString();
            }

            foreach (OptionFormat option in category.Options) {
                msg+="\t";
                StyleBuilder builder = new StyleBuilder().Foreground((StandardColor)StandardColor.Colors.BrightGreen).Text("[");
                for (int i = 0; i < option.keys.Count; i++) {
                    string key = option.keys[i];
                    builder.Bold().Text((key.Length > 1 ? "--" : "-")+key).Bold(false);
                    if (i < option.keys.Count-1) {
                        builder.Text(", ");
                    }
                }
                builder.Text("]").ResetForeground().Text(": "+option.description).NewLine();
                foreach (OptionFormat.ParameterFormat parameter in option.parameters) {
                    builder.Text("\t\t").Foreground(RGBColor.Orange).Text(parameter.name).ResetForeground().Text(": "+parameter.description).NewLine();
                }
                msg+=builder;
            }
            msg+="\n";
        }
        // if (Format.AllArguments.Count > 0) msg += "Required arguments:\n";
        // foreach (ArgumentFormat argument in Format.AllArguments) {
        //     msg += $"\t[{argument.name}]: {argument.description}\n";
        // }
        
        // if (Format.AllOptions.Count > 0) msg += "Options:\n";
        // foreach (OptionFormat option in Format.AllOptions) {
        //     msg += "\t[";
        //     for (int i = 0; i < option.keys.Count; i++) {
        //         string key = option.keys[i];
        //         msg += (key.Length > 1 ? "--" : "-") + key;
        //         if (i < option.keys.Count-1) {
        //             msg += ", ";
        //         }
        //     }
        //     msg += $"]: {option.description}\n";
        //     foreach (OptionFormat.ParameterFormat parameter in option.parameters) {
        //         msg += $"\t\t[{parameter.name}]: {parameter.description}\n";
        //     }
        // }

        Terminal.Write(msg);
    }

    /// <summary>
    /// Displays the version of the program to the screen.
    /// </summary>
    /// <param name="showName">If the name and version of the program should be shown.</param>
    /// <param name="showDescription">if the description of the program should be shown.</param>
    public void ShowVersion(bool showName = true, bool showDescription = true) {
        string msg = "";
        if (showName) {
            string name = GetName();
            msg = name == "" ? "" : name+"\n";
        }
        // Description
        if (showDescription) {
            if (Format.description != null) msg += "\n"+Format.description;
        }

        if (msg != "") Terminal.WriteLine(msg);
    }

    private string GetName() {
        string name = "";
        if (Format.name != null) name+=Format.name;
        if (Format.version != null) {
            if (name != "") name+=" ";
            name+=Format.version;
        }

        return name;
    }
    
}