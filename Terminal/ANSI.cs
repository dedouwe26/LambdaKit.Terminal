using System.Diagnostics.Contracts;

namespace OxDED.Terminal;

/// <summary>
/// Contains all the ANSI codes.
/// </summary>
public static class ANSI {
    #region C0 - Control Codes
    /// <summary>
    /// Bell, Makes and audible noise.
    /// </summary>
    public const string BEL = "\x07";
    /// <summary>
    /// Escape, Starts an escape sequence.
    /// </summary>
    public const string ESC = "\x1B";
    #endregion

    #region C1 - Control Codes
    /// <summary>
    /// Control Sequence Introducer, starts useful sequences.
    /// </summary>
    public const string CSI = ESC + "[";
    /// <summary>
    /// String Terminator, ends strings in some sequences.
    /// </summary>
    public const string ST = ESC + "\\";
    /// <summary>
    /// Operating System Command, starts sequences.
    /// </summary>
    public const string OSC = ESC + "]";
    #endregion

    #region CSI - Control Sequence Introducer
    /// <summary>
    /// Device Status Report, report the cursor position in the format CSI+<c>r;cR</c> where r = row and c = column.
    /// </summary>
    public const string DSR = CSI + "6n";
    /// <summary>
    /// CUrsor Position, moves the cursor to that position.
    /// </summary>
    /// <param name="row">The new row of the cursor (0-based).</param>
    /// <param name="col">The new column of the cursor (0-based).</param>
    /// <returns>The escape sequence.</returns>
    [Pure]
    public static string CUP(uint row, uint col) => CSI + $"{row + 1};{col + 1}H";
    /// <summary>
    /// Erase in Display (All), clears everything, including the scrollback buffer (xterm).
    /// </summary>
    public const string EDA = CSI+"3J";
    /// <summary>
    /// Erase in Display (Screen), clears the whole screen (cursor pos. unknown).
    /// </summary>
    public const string EDS = CSI+"2J";
    /// <summary>
    /// Erase in Display (Cursor Start), clears everything from the cursor to the beginning of the screen.
    /// </summary>
    public const string EDCS = CSI+"1J";
    /// <summary>
    /// Erase in Display (Cursor End), clears everything from the cursor to the end of the screen.
    /// </summary>
    public const string EDCE = CSI+"J";
    /// <summary>
    /// Erase in Line (All), clears the entire line.
    /// </summary>
    public const string ELA = CSI+"2K";
    /// <summary>
    /// Erase in Line (Cursor Start), clears everything from the cursor to the beginning of the line.
    /// </summary>
    public const string ELCS = CSI+"1K";
    /// <summary>
    /// Erase in Line (Cursor End), Clears everything from the cursor to the end of the line.
    /// </summary>
    public const string ELCE = CSI+"K";
    /// <summary>
    /// Scroll Up, scrolls up. Adds new lines on the bottom.
    /// </summary>
    /// <param name="amount">How many lines to scroll up.</param>
    /// <returns>The escape sequence.</returns>
    public static string SU(uint amount = 1) {
        return CSI+amount.ToString()+"S";
    }
    /// <summary>
    /// Scroll Down, scrolls down. Adds new lines on the top.
    /// </summary>
    /// <param name="amount">How many lines to scroll down.</param>
    /// <returns>The escape sequence.</returns>
    public static string SD(uint amount = 1) {
        return CSI+amount.ToString()+"T";
    }
    /// <summary>
    /// DECTCEM, makes the cursor visible (VT220).
    /// </summary>
    /// <seealso cref="CursorInvisible"/>
    public const string CursorVisible = CSI+"?25h";
    /// <summary>
    /// DECTCEM, makes the cursor invisible (VT220).
    /// </summary>
    /// <seealso cref="CursorVisible"/>
    public const string CursorInvisible = CSI+"?25l";

    /// <summary>
    /// Uses the alternative screen buffer from XTerm. In the DEC Private Mode Set.
    /// </summary>
    public const string UseAlternateScreenBuffer = CSI+"?1047h";
    /// <summary>
    /// Uses the normal screen buffer from XTerm. In the DEC Private Mode Set.
    /// </summary>
    public const string UseNormalScreenBuffer = CSI+"?1047l";
    /// <summary>
    /// Uses the alternative screen buffer and saves the cursor (<see cref="DECSC"/>), from XTerm. In the DEC Private Mode Set.
    /// </summary>
    public const string SaveCursorAndAltScreen = CSI+"?1049h";
    /// <summary>
    /// Uses the normal screen buffer and restores the cursor (<see cref="DECRC"/>), from XTerm. In the DEC Private Mode Set.
    /// </summary>
    public const string RestoreCursorAndNormScreen = CSI+"?1049l";
    /// <summary>
    /// Enables bracketed paste mode from XTerm. Transmits <c>ESC[200~ ... ESC[201~</c>.
    /// </summary>
    public const string BracketedPasteOn = CSI+"?2004h";
    /// <summary>
    /// Disables <see cref="BracketedPasteOn"/> from XTerm.
    /// </summary>
    public const string BracketedPasteOff = CSI+"?2004l";
    #endregion

    #region OSC - Operating System Command
    /// <summary>
    /// Creates a hyperlink escape sequence from (OSC 8).
    /// </summary>
    /// <param name="URI">The destination of the hyperlink.</param>
    /// <param name="label">The label of the hyperlink.</param>
    /// <param name="parameters">The optional parameters of the hyperlink.</param>
    /// <returns>The escape sequence.</returns>
    public static string Hyperlink(string URI, string label, string parameters = "") {
        return OSC+"8;"+parameters+";"+URI+ST+label+OSC+"8;;"+ST;
    }
    #endregion

    #region Fp - Escape sequences
    /// <summary>
    /// DEC Save Cursor, saves the cursor position.
    /// </summary>
    public const string DECSC = ESC+"7";
    /// <summary>
    /// DEC Restore Cursor, restores the cursor position from <see cref="DECSC"/>.
    /// </summary>
    public const string DECRC = ESC+"8";
    #endregion

    /// <summary>
    /// Select Graphic Rendition, contains all graphical sequences.
    /// </summary>
    public static class SGR {
        /// <summary>
        /// Makes a control sequence from the given SGR code.
        /// </summary>
        /// <param name="code">The given SGR code.</param>
        /// <returns>The escape sequence.</returns>
        public static string Build(string code) {
            if (code.Length < 1) return "";
            return CSI + code + 'm';
        }
        /// <summary>
        /// Makes a control sequence from the given SGR codes.
        /// </summary>
        /// <param name="codes">The given SGR codes.</param>
        /// <returns>The escape sequence.</returns>
        public static string Build(params string[] codes) {
            if (codes.Length < 1) return "";
            return CSI + string.Join(';', codes) + 'm';
        }
        
        #region SGR - Styles
        /// <summary>
        /// Resets all styles and colors.
        /// </summary>
        public const string RESETALL = "0";
        /// <summary>
        /// Resets all styles and colors (ready for use).
        /// </summary>
        public const string BuildedResetAll = CSI+RESETALL+"m";
        /// <summary>
        /// Increases intensity or color. Interferes with <see cref="FAINT"/>.
        /// </summary>
        public const string BOLD = "1";
        /// <summary>
        /// Decreases intensity or color. Interferes with <see cref="BOLD"/>.
        /// </summary>
        public const string FAINT = "2";
        /// <summary>
        /// Makes the text italic. Not widely supported.
        /// </summary>
        public const string ITALIC = "3";
        /// <summary>
        /// Adds an underline to the text. Interferes with <see cref="DOUBLEUNDERLINE"/>.
        /// </summary>
        public const string UNDERLINE = "4";
        /// <summary>
        /// Blinks slowly. More widly implemented than <see cref="RAPIDBLINK"/>. Interferes with <see cref="RAPIDBLINK"/>.
        /// </summary>
        public const string SLOWBLINK = "5";
        /// <summary>
        /// Blinks fast. Not widely supported. Interferes with <see cref="SLOWBLINK"/>.
        /// </summary>
        public const string RAPIDBLINK = "6";
        /// <summary>
        /// Swap fore- and background colors.
        /// </summary>
        public const string INVERT = "7";
        /// <summary>
        /// Hides the text. Not widely supported.
        /// </summary>
        public const string HIDE = "8";
        /// <summary>
        /// Crosses out text.
        /// </summary>
        public const string STRIKETHROUGH = "9";
        /// <summary>
        /// Resets the font to default.
        /// </summary>
        public const string DEFAULTFONT = "10";
        /// <summary>
        /// Selects an alternative font (0 to 9). Where 9 is Fraktur, which is rarely supported.
        /// </summary>
        /// <param name="font">The font to use (0 to 9). Where 9 is Fraktur, which is rarely supported.</param>
        /// <returns>The partial escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Pure]
        public static string Font(byte font) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(font, 9, nameof(font));
            return (font+11).ToString();
        }
        /// <summary>
        /// Adds a double underline to the text, sometimes disables <see cref="BOLD"/>. Not widely supported. Interferes with <see cref="UNDERLINE"/>
        /// </summary>
        public const string DOUBLEUNDERLINE = "21";
        /// <summary>
        /// Adds a line above the text.
        /// </summary>
        public const string OVERLINE = "53";
        /// <summary>
        /// Resets <see cref="BOLD"/> and <see cref="FAINT"/>.
        /// </summary>
        public const string RESETINTENSITY = "22";
        /// <summary>
        /// Resets <see cref="ITALIC"/>.
        /// </summary>
        public const string RESETITALIC = "23";
        /// <summary>
        /// Resets <see cref="UNDERLINE"/> and <see cref="DOUBLEUNDERLINE"/>. Not to be confused with <see cref="DEFAULTUNDERLINE"/>.
        /// </summary>
        public const string RESETUNDERLINE = "24";
        /// <summary>
        /// Resets <see cref="OVERLINE"/>.
        /// </summary>
        public const string RESETOVERLINE = "55";
        /// <summary>
        /// Resets <see cref="SLOWBLINK"/> and <see cref="RAPIDBLINK"/>.
        /// </summary>
        public const string RESETBLINK = "25";
        /// <summary>
        /// Resets <see cref="INVERT"/>.
        /// </summary>
        public const string RESETINVERT = "27";
        /// <summary>
        /// Resets <see cref="HIDE"/>.
        /// </summary>
        public const string RESETHIDE = "28";
        /// <summary>
        /// Resets <see cref="STRIKETHROUGH"/>.
        /// </summary>
        public const string RESETSTRIKETROUGH = "29";
        #endregion

        #region SGR - Colors
        /// <summary>
        /// Resets the foreground color to default.
        /// </summary>
        public const string DEFAULTFOREGROUND = "39";
        /// <summary>
        /// Sets the standard foreground color (3-bit).
        /// </summary>
        /// <param name="standardColor">The color to use (3-bit) (0 to 7).</param>
        /// <returns>A partial escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Pure]
        public static string StandardForeground(byte standardColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(standardColor, 7, nameof(standardColor));
            return (30 + standardColor).ToString();
        }
        /// <summary>
        /// Sets a bright foreground color (4-bit).
        /// </summary>
        /// <param name="brightColor">The bright color to use (4-bit) (0 to 7).</param>
        /// <returns>A partial escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Pure]
        public static string BrightForeground(byte brightColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(brightColor, 7, nameof(brightColor));
            return (90 + brightColor).ToString();
        }
        /// <summary>
        /// Sets the specified foreground color (8-bit).
        /// </summary>
        /// <param name="specifiedColor">The color to use.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedForeground(byte specifiedColor) {
            return "38;5;" + specifiedColor;
        }
        /// <summary>
        /// Sets the specified foreground color (24-bit).
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedForeground(byte r, byte g, byte b) {
            return "38;2;" + r + ";" + g + ";" + b;
        }
        /// <summary>
        /// Resets the background color to default.
        /// </summary>
        public const string DEFAULTBACKGROUND = "49";
        /// <summary>
        /// Sets the standard background color (3-bit).
        /// </summary>
        /// <param name="standardColor">The color to use (3-bit) (0 to 7).</param>
        /// <returns>A partial escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Pure]
        public static string StandardBackground(byte standardColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(standardColor, 7, nameof(standardColor));
            return (40 + standardColor).ToString();
        }
        /// <summary>
        /// Sets a bright background color (4-bit).
        /// </summary>
        /// <param name="brightColor">The bright color to use (4-bit) (0 to 7).</param>
        /// <returns>A partial escape sequence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Pure]
        public static string BrightBackground(byte brightColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(brightColor, 7, nameof(brightColor));
            return (100 + brightColor).ToString();
        }
        /// <summary>
        /// Sets the specified background color (8-bit).
        /// </summary>
        /// <param name="specifiedColor">The color to use.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedBackground(byte specifiedColor) {
            return "48;5;" + specifiedColor;
        }
        /// <summary>
        /// Sets the specified foreground color (24-bit).
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedBackground(byte r, byte g, byte b) {
            return "48;2;" + r + ";" + g + ";" + b;
        }

        /// <summary>
        /// Resets the underline color to default.
        /// </summary>
        public const string DEFAULTUNDERLINE = "59";
        /// <summary>
        /// Sets the specified foreground color (8-bit).
        /// </summary>
        /// <param name="specifiedColor">The color to use.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedUnderline(byte specifiedColor) {
            return "48;5;" + specifiedColor;
        }
        /// <summary>
        /// Sets the specified foreground color (24-bit).
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <returns>A partial escape sequence.</returns>
        [Pure]
        public static string SpecifiedUnderline(byte r, byte g, byte b) {
            return "58;2;" + r + ";" + g + ";" + b;
        }
        #endregion
    }
    
}
