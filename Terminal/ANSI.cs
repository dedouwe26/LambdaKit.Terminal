using System.Diagnostics.Contracts;

namespace OxDED.Terminal;

/// <summary>
/// Contains all the ANSI codes.
/// </summary>
public static class ANSI {
    #region C0 - Control Codes
    public const string BEL = "\x07";
    public const string ESC = "\x1B";
    #endregion

    #region C1 - Control Codes
    public const string CSI = ESC + "\x9B";
    #endregion

    #region CSI - Control Sequence Introducer
    public const string DSR = CSI + "6n";
    [Pure]
    public static string CUP(uint row, uint col) => CSI + $"{row + 1};{col + 1}H";
    // TODO: Add ED.
    #endregion

    public static class SGR {
        /// <summary>
        /// Makes a control sequence from the given SGR codes.
        /// </summary>
        /// <param name="codes">The codes</param>
        /// <returns></returns>
        public static string Build(params string[] codes) {
            return CSI + string.Join(';', codes) + 'm';
        }
        #region SGR - Styles
        /// <summary>
        /// Resets all styles and colors.
        /// </summary>
        public const string RESETALL = "0";
        public const string BOLD = "1";
        public const string FAINT = "2";
        public const string ITALIC = "3";
        public const string UNDERLINE = "4";
        /// <summary>
        /// Blinks slowly. More widly implemented than <see cref="RAPIDBLINK"/>. Interferes with <see cref="RAPIDBLINK"/>.
        /// </summary>
        public const string SLOWBLINK = "5";
        public const string RAPIDBLINK = "6";
        public const string INVERT = "7";
        public const string HIDE = "8";
        public const string STRIKETHROUGH = "9";
        public const string DEFAULTFONT = "10";
        [Pure]
        public static string Font(uint font) => font.ToString();
        public const string DOUBLEUNDERLINE = "21";
        public const string OVERLINE = "53";
        public const string RESETINTENSITY = "22";
        public const string RESETITALIC = "23";
        /// <summary>
        /// Resets underline or double underline. Not to be confused with <see cref="DEFAULTUNDERLINE"/>.
        /// </summary>
        public const string RESETUNDERLINE = "24";
        public const string RESETOVERLINE = "55";
        public const string RESSETBLINK = "25";
        public const string RESSETINVERT = "27";
        public const string RESETHIDE = "28";
        #endregion

        #region SGR - Colors
        public const string DEFAULTFOREGROUND = "39";
        [Pure]
        public static string StandardForeground(byte standardColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(standardColor, 7, nameof(standardColor));
            return (30 + standardColor).ToString();
        }
        [Pure]
        public static string BrightForeground(byte brightColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(brightColor, 7, nameof(brightColor));
            return (90 + brightColor).ToString();
        }
        [Pure]
        public static string SpecifiedForeground(byte specifiedColor) {
            return "38;5;" + specifiedColor;
        }
        [Pure]
        public static string SpecifiedForeground(byte r, byte g, byte b) {
            return "38;2;" + r + ";" + g + ";" + b;
        }
        public const string DEFAULTBACKGROUND = "49";
        [Pure]
        public static string StandardBackground(byte standardColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(standardColor, 7, nameof(standardColor));
            return (40 + standardColor).ToString();
        }
        [Pure]
        public static string BrightBackground(byte brightColor) {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(brightColor, 7, nameof(brightColor));
            return (100 + brightColor).ToString();
        }
        [Pure]
        public static string SpecifiedBackground(byte specifiedColor) {
            return "48;5;" + specifiedColor;
        }
        [Pure]
        public static string SpecifiedBackground(byte r, byte g, byte b) {
            return "48;2;" + r + ";" + g + ";" + b;
        }
        public const string DEFAULTUNDERLINE = "59";
        [Pure]
        public static string SpecifiedUnderline(byte specifiedColor) {
            return "48;5;" + specifiedColor;
        }
        [Pure]
        public static string SpecifiedUnderline(byte r, byte g, byte b) {
            return "58;2;" + r + ";" + g + ";" + b;
        }
        #endregion
    }
    
}
