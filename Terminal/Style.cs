namespace OxDED.Terminal;

/// <summary>
/// Contains the style decorations.
/// </summary>
public class Style : IEquatable<Style>, ICloneable {
    /// <summary>
    /// Increases intensity or color. Interferes with <see cref="Faint"/>.
    /// </summary>
    public bool Bold = false;
    /// <summary>
    /// Decreases intensity or color. Interferes with <see cref="Bold"/>.
    /// </summary>
    public bool Faint = false;
    /// <summary>
    /// Makes the text italic. Not widely supported.
    /// </summary>
    public bool Italic = false;
    /// <summary>
    /// Adds an underline to the text. Interferes with <see cref="DoubleUnderline"/>.
    /// </summary>
    public bool Underline = false;
    /// <summary>
    /// Blinks slowly. More widly implemented than <see cref="RapidBlink"/>. Interferes with <see cref="RapidBlink"/>.
    /// </summary>
    public bool Blink = false;
    /// <summary>
    /// Blinks fast. Not widely supported. Interferes with <see cref="Blink"/>.
    /// </summary>
    public bool RapidBlink = false;
    /// <summary>
    /// Swap fore- and background colors.
    /// </summary>
    public bool Inverse = false;
    /// <summary>
    /// Hides the text. Not widely supported.
    /// </summary>
    public bool Invisible = false;
    /// <summary>
    /// Crosses out text.
    /// </summary>
    public bool Striketrough = false;
    /// <summary>
    /// Adds a double underline to the text, sometimes disables <see cref="Bold"/>. Not widely supported. Interferes with <see cref="Underline"/>
    /// </summary>
    public bool DoubleUnderline = false;
    /// <summary>
    /// Adds a line above the text.
    /// </summary>
    public bool Overline = false;
    private byte? font = null;
    /// <summary>
    /// Which alternative font to use (0 to 9), null for default. Where 9 is Fraktur, which is rarely supported.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public byte? Font { get => font; set {
        if (value == null) { font = value; return; }
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Value, 9);
        font = value;
    } }

    /// <summary>
    /// The text color.
    /// </summary>
    public IColor? ForegroundColor = null;
    /// <summary>
    /// The color behind the text.
    /// </summary>
    public IColor? BackgroundColor = null;
    /// <summary>
    /// The color behind the text.
    /// </summary>
    public ISpecifiedColor? UnderlineColor = null;

    /// <summary>
    /// Creates a basic style.
    /// </summary>
    public Style() { }

    /// <summary>
    /// Creates an ANSI coded string for the chosen decorations.
    /// </summary>
    /// <param name="reset">If it should reset a style when it is disabled.</param>
    /// <returns>The ANSI string.</returns>
    public string ToANSI(bool reset = true) {
        List<string> codes = [];
        if (Bold) codes.Add(ANSI.SGR.BOLD);
        if (Faint) codes.Add(ANSI.SGR.FAINT);
        if (Underline) codes.Add(ANSI.SGR.UNDERLINE);
        if (DoubleUnderline) codes.Add(ANSI.SGR.DOUBLEUNDERLINE);
        if (Blink) codes.Add(ANSI.SGR.SLOWBLINK);
        if (RapidBlink) codes.Add(ANSI.SGR.RAPIDBLINK);
        if (Italic) codes.Add(ANSI.SGR.ITALIC);
        if (Inverse) codes.Add(ANSI.SGR.INVERT);
        if (Invisible) codes.Add(ANSI.SGR.HIDE);
        if (Striketrough) codes.Add(ANSI.SGR.STRIKETHROUGH);
        if (Overline) codes.Add(ANSI.SGR.OVERLINE);
        if (font!=null) codes.Add(ANSI.SGR.Font(font.Value));
        if (BackgroundColor!=null) codes.Add(BackgroundColor.ToBackgroundSGR());
        if (ForegroundColor!=null) codes.Add(ForegroundColor.ToForegroundSGR());
        if (UnderlineColor!=null) codes.Add(UnderlineColor.ToUnderlineSGR());
        return (reset ? ANSI.SGR.BuildedResetAll : "") + ANSI.SGR.Build([.. codes]);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Checks if the that color is identical to this one.
    /// </remarks>
    public bool Equals(Style? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return (Bold == other.Bold) &&
               (Faint == other.Faint) &&
               (Italic == other.Italic) &&
               (Underline == other.Underline) &&
               (Blink == other.Blink) &&
               (RapidBlink == other.RapidBlink) &&
               (Inverse == other.Inverse) &&
               (Invisible == other.Invisible) &&
               (Striketrough == other.Striketrough) &&
               (DoubleUnderline == other.DoubleUnderline) &&
               (Overline == other.Overline) &&
               (font == other.font) &&
               (ForegroundColor == other.ForegroundColor) &&
               (BackgroundColor == other.BackgroundColor) &&
               (UnderlineColor == other.UnderlineColor);
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Checks if the that color is identical to this one.
    /// </remarks>
    public override bool Equals(object? obj) {
        return Equals(obj as Style);
    }
    /// <inheritdoc/>
    public override int GetHashCode() {
        int hash = 0;
        if (Bold) hash |= 1<<0;
        if (Faint) hash |= 1<<1;
        if (Italic) hash |= 1<<2;
        if (Underline) hash |= 1<<3;
        if (Blink) hash |= 1<<4;
        if (RapidBlink) hash |= 1<<5;
        if (Inverse) hash |= 1<<6;
        if (Invisible) hash |= 1<<7;
        if (Striketrough) hash |= 1<<9;
        if (DoubleUnderline) hash |= 1<<10;
        if (Overline) hash |= 1<<11;
        hash ^= font.GetHashCode();
        if (BackgroundColor!=null) hash ^= BackgroundColor.GetHashCode();
        if (ForegroundColor!=null)hash ^= ForegroundColor.GetHashCode();
        if (UnderlineColor!=null) hash ^= UnderlineColor.GetHashCode();
        return hash;
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Calls <see cref="CloneStyle"/>.
    /// </remarks>
    public object Clone() {
        return CloneStyle();
    }
    /// <summary>
    /// Clones this style.
    /// </summary>
    /// <returns>The new copy of this style.</returns>
    public Style CloneStyle() {
        return new Style {
            Bold = Bold,
            Faint = Faint,
            Italic = Italic,
            Underline = Underline,
            Blink = Blink,
            RapidBlink = RapidBlink,
            Inverse = Inverse,
            Invisible = Invisible,
            Striketrough = Striketrough,
            DoubleUnderline = DoubleUnderline,
            Overline = Overline,
            font = font,
            ForegroundColor = ForegroundColor?.CloneColor(),
            BackgroundColor = BackgroundColor?.CloneColor()
        };
    }
    /// <inheritdoc/>
    public override string ToString() {
        return ToANSI();
    }
}