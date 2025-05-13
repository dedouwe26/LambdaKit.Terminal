using System.Collections.ObjectModel;

namespace LambdaKit.Terminal.Arguments;

/// <summary>
/// Represents a format for arguments and options.
/// </summary>
public class ArgumentFormatter {
    /// <summary>
    /// Whether the program should quit when a parsing error occured.
    /// </summary>
    public bool shouldExitOnError = true;
    /// <summary>
    /// The name of this application. Used by the version menu and help menu.
    /// </summary>
    public string? name;
    /// <summary>
    /// The description of this application. Used by the version menu and help menu.
    /// </summary>
    public string? description;
    /// <summary>
    /// The version of this application. Used by the version menu and help menu.
    /// </summary>
    public string? version;
    
    /// <summary>
    /// All the categories.
    /// </summary>
    public readonly List<CategoryFormat> Categories = [];

    internal List<ArgumentFormat>? allArguments;
    /// <summary>
    /// All the arguments in all categories.
    /// </summary>
    public ReadOnlyCollection<ArgumentFormat> AllArguments { get {
        if (allArguments == null) {
            allArguments = [];
            foreach (CategoryFormat category in Categories) {
                allArguments.AddRange(category.Arguments);
            }
        }
        return allArguments.AsReadOnly();
    } }

    internal List<OptionFormat>? allOptions;
    /// <summary>
    /// All the arguments in all categories.
    /// </summary>
    public ReadOnlyCollection<OptionFormat> AllOptions { get {
        if (allOptions == null) {
            allOptions = [];
            foreach (CategoryFormat category in Categories) {
                allOptions.AddRange(category.Options);
            }
        }
        return allOptions.AsReadOnly();
    } }

    private CategoryFormat? generalCategory;
    /// <summary>
    /// The general category. It is going to be created when you call the get method.
    /// </summary>
    public CategoryFormat GeneralCategory { get {
        return generalCategory ??= new(this, "General");
    } }

    /// <summary>
    /// Uses the general category.
    /// </summary>
    /// <returns>The category format of the general category.</returns>
    public CategoryFormat General() {
        return GeneralCategory;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public CategoryFormat Category() {
        return new CategoryFormat(this);
    }

    // /// <summary>
    // /// Creates a new argument format.
    // /// </summary>
    // public ArgumentFormatter() {
    // }

    /// <summary>
    /// Sets whether the program should quit when a parsing error occured.
    /// </summary>
    /// <param name="shouldExit">True if it should quit.</param>
    /// <returns>This argument formatter.</returns>
    public ArgumentFormatter ShouldExitOnError(bool shouldExit) {
        shouldExitOnError = shouldExit;
        return this;
    }
    /// <summary>
    /// Sets the name of this application. Used by the version menu and help menu.
    /// </summary>
    /// <param name="name">The name of the application.</param>
    /// <returns>This argument formatter.</returns>
    public ArgumentFormatter Name(string? name) {
        this.name = name;
        return this;
    }
    /// <summary>
    /// Sets the description of this application. Used by the version menu and help menu.
    /// </summary>
    /// <param name="description">The description of the application.</param>
    /// <returns>This argument formatter.</returns>
    public ArgumentFormatter Description(string? description) {
        this.description = description;
        return this;
    }
    /// <summary>
    /// Sets the version of this application. Used by the version menu and help menu.
    /// </summary>
    /// <param name="version">The version of the application.</param>
    /// <returns>This argument formatter.</returns>
    public ArgumentFormatter Version(string? version) {
        this.version = version;
        return this;
    }

    /// <summary>
    /// The currently used help option.
    /// </summary>
    public HelpOptionFormat? CurrentHelpOption { get; internal set; }
    /// <summary>
    /// The currently used version option.
    /// </summary>
    public VersionOptionFormat? CurrentVersionOption { get; internal set; }

    /// <summary>
    /// Finishes formatting and creates a parser.
    /// </summary>
    /// <returns>A new argument parser.</returns>
    public ArgumentParser Finish() {
        return new(this);
    }
}

/// <summary>
/// A argument and option category. Used in the help menu.
/// </summary>
public class CategoryFormat {
    /// <summary>
    /// The name of the category.
    /// </summary>
    public string name;
    /// <summary>
    /// The arguments of this category.
    /// </summary>
    public readonly List<ArgumentFormat> Arguments = [];
    /// <summary>
    /// The arguments of this category.
    /// </summary>
    public readonly List<OptionFormat> Options = [];
    /// <summary>
    /// The parent format.
    /// </summary>
    public readonly ArgumentFormatter Parent;

    internal CategoryFormat(ArgumentFormatter parent, string name = "Unnamed") {
        Parent = parent;
        this.name = name;
    }

    /// <summary>
    /// Sets the name of the category.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public CategoryFormat Name(string name) {
        this.name = name;
        return this;
    } 
    /// <summary>
    /// Creates a new option.
    /// </summary>
    /// <returns>The new option format.</returns>
    public OptionFormat Option() {
        return new(this);
    }
    /// <summary>
    /// Creates a new argument.
    /// </summary>
    /// <returns>The new argument format.</returns>
    public ArgumentFormat Argument() {
        return new(this);
    }

    /// <summary>
    /// Sets a new help option.
    /// </summary>
    /// <returns>The help option format.</returns>
    public HelpOptionFormat HelpOption() {
        return new HelpOptionFormat(this);
    }
    /// <summary>
    /// Sets a new version option.
    /// </summary>
    /// <returns>The version option format.</returns>
    public VersionOptionFormat VersionOption() {
        return new VersionOptionFormat(this);
    }

    /// <summary>
    /// Finishes and saves this category format.
    /// </summary>
    /// <returns>The parent.</returns>
    public ArgumentFormatter Finish() {
        if (!Parent.Categories.Exists((CategoryFormat c) => ReferenceEquals(this, c)))
            Parent.Categories.Add(this);
        return Parent;
    }
}

/// <summary>
/// Represents a format for an argument.
/// </summary>
public class ArgumentFormat {
    /// <summary>
    /// The description of this argument. Used by the help menu.
    /// </summary>
    public string description = "";
    
    /// <summary>
    /// The name of this argument. Used by the help menu.
    /// </summary>
    public string name = "";
    
    /// <summary>
    /// The parent format.
    /// </summary>
    public readonly CategoryFormat Parent;

    internal ArgumentFormat(CategoryFormat parent) {
        Parent = parent;
    }
    /// <summary>
    /// Sets the name of this argument.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <returns>This argument.</returns>
    public ArgumentFormat Name(string name) {
        this.name = name;
        return this;
    }
    /// <summary>
    /// Sets the description of this argument.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <returns>This argument.</returns>
    public ArgumentFormat Description(string description) {
        this.description = description;
        return this;
    }

    /// <summary>
    /// Finishes and saves this argument format.
    /// </summary>
    /// <returns>The parent.</returns>
    public CategoryFormat Finish() {
        Parent.Arguments.Add(this);
        Parent.Parent.allArguments = null;
        return Parent;
    }
}

/// <summary>
/// Represents a format for an option.
/// </summary>
public class OptionFormat {
    /// <summary>
    /// All the keys of this option.
    /// </summary>
    public readonly List<string> keys = [];
    /// <summary>
    /// The description of this argument. Used by the help menu.
    /// </summary>
    public string description = "";
    /// <summary>
    /// All the parameters of this option.
    /// </summary>
    public readonly List<ParameterFormat> parameters = [];

    /// <summary>
    /// The parent format.
    /// </summary>
    public readonly CategoryFormat Parent;

    internal OptionFormat(CategoryFormat parent) {
        Parent = parent;
    }

    /// <summary>
    /// Adds a key to check for.
    /// </summary>
    /// <param name="key">The new key.</param>
    /// <returns>This option.</returns>
    public virtual OptionFormat Key(string key) {
        keys.Add(key);
        return this;
    }
    /// <summary>
    /// Adds keys to check for.
    /// </summary>
    /// <param name="keys">The new keys to add.</param>
    /// <returns>This option.</returns>
    public virtual OptionFormat Keys(IEnumerable<string> keys) {
        this.keys.AddRange(keys);
        return this;
    }
    /// <summary>
    /// Sets the description of this option.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <returns>This option.</returns>
    public virtual OptionFormat Description(string description) {
        this.description = description;
        return this;
    }
    /// <summary>
    /// Adds a new parameter to this option.
    /// </summary>
    /// <returns>The parameter format.</returns>
    public virtual ParameterFormat Parameter() {
        return new(this);
    }

    /// <summary>
    /// Finishes and saves this option format.
    /// </summary>
    /// <returns>The parent.</returns>
    public virtual CategoryFormat Finish() {
        Parent.Options.Add(this);
        Parent.Parent.allOptions = null;
        return Parent;
    }
    /// <summary>
    /// Represents a format for an option's parameter.
    /// </summary>
    public class ParameterFormat {
        /// <summary>
        /// The name of this parameter. Used by the help menu.
        /// </summary>
        public string name = "";
        /// <summary>
        /// The description of this argument. Used by the help menu.
        /// </summary>
        public string description = "";

        /// <summary>
        /// The parent format.
        /// </summary>
        public readonly OptionFormat OptionFormat;

        internal ParameterFormat(OptionFormat optionBuilder) {
            OptionFormat = optionBuilder;
        }

        /// <summary>
        /// Sets the name of this parameter.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <returns>This parameter.</returns>
        public ParameterFormat Name(string name) {
            this.name = name;
            return this;
        }
        /// <summary>
        /// Sets the description of this parameter.
        /// </summary>
        /// <param name="description">The new description.</param>
        /// <returns>This parameter.</returns>
        public ParameterFormat Description(string description) {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Finishes and saves this parameter format.
        /// </summary>
        /// <returns>The parent.</returns>
        public OptionFormat Finish() {
            OptionFormat.parameters.Add(this);
            return OptionFormat;
        }
    }
}

/// <summary>
/// Represents a configurable help option.
/// </summary>
public class HelpOptionFormat : OptionFormat {
    internal HelpOptionFormat(CategoryFormat argumentFormatter) : base(argumentFormatter) {
        Keys(["h", "help"]).Description("Displays this help page.");
    }
    /// <summary>
    /// Whether the program should quit when this option is used or not.
    /// </summary>
    public bool quit = true;
    /// <summary>
    /// Sets whether the program should quit when this option is used or not.
    /// </summary>
    /// <param name="quit">Whether the program should quit when this option is used or not.</param>
    /// <returns>This option.</returns>
    public HelpOptionFormat Quit(bool quit) {
        this.quit = quit;
        return this;
    }
    /// <inheritdoc/>
    public override CategoryFormat Finish() {
        Parent.Parent.CurrentHelpOption = this;
        return base.Finish();
    }
}

/// <summary>
/// Represents a configurable version option.
/// </summary>
public class VersionOptionFormat : OptionFormat {
    internal VersionOptionFormat(CategoryFormat argumentFormatter) : base(argumentFormatter) {
        Keys(["v", "version"]).Description("Displays the version and description of this program.");
    }
    /// <summary>
    /// Whether the program should quit when this option is used or not.
    /// </summary>
    public bool quit = true;
    /// <summary>
    /// Sets whether the program should quit when this option is used or not.
    /// </summary>
    /// <param name="quit">Whether the program should quit when this option is used or not.</param>
    /// <returns>This option.</returns>
    public VersionOptionFormat Quit(bool quit) {
        this.quit = quit;
        return this;
    }
    /// <inheritdoc/>
    public override CategoryFormat Finish() {
        Parent.Parent.CurrentVersionOption = this;
        return base.Finish();
    }
}