namespace OxDED.Terminal;

/// <summary>
/// Creates stylized text.
/// </summary>
public class StyleBuilder {
    private Style style = new();
    private string text; 

    /// <summary>
    /// Creates a new style builder.
    /// </summary>
    public StyleBuilder() {
        text = "";
    }
    /// <summary>
    /// Writes the text bold or not.
    /// </summary>
    /// <param name="isBold">Whether the text should be bold.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Bold(bool isBold = true) {
        style.Bold = isBold;
        if (!(isBold||style.RapidBlink)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETINTENSITY);
        }
        return this;
    }
    /// <summary>
    /// Writes the text faint or not.
    /// </summary>
    /// <param name="isFaint">Whether the text should be faint.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Faint(bool isFaint = true) {
        style.Faint = isFaint;
        if (!(isFaint||style.Bold)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETINTENSITY);
        }
        return this;
    }
    /// <summary>
    /// Writes the text italic or not.
    /// </summary>
    /// <param name="isItalic">Whether the text should be italic.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Italic(bool isItalic = true) {
        style.Italic = isItalic;
        if (!isItalic) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETITALIC);
        }
        return this;
    }
    /// <summary>
    /// Writes the text underlined or not.
    /// </summary>
    /// <param name="isUnderlined">Whether the text should be underlined.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Underline(bool isUnderlined = true) {
        style.Underline = isUnderlined;
        if (!(isUnderlined||style.DoubleUnderline)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETUNDERLINE);
        }
        return this;
    }
    /// <summary>
    /// Writes the text blinking or not.
    /// </summary>
    /// <param name="isBlinking">Whether the text should be blinking.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Blink(bool isBlinking = true) {
        style.Blink = isBlinking;
        if (!(isBlinking||style.RapidBlink)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETBLINK);
        }
        return this;
    }
    /// <summary>
    /// Writes the text rapidly blinking or not.
    /// </summary>
    /// <param name="isBlinking">Whether the text should be rapidly blinking.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder RapidBlink(bool isBlinking = true) {
        style.RapidBlink = isBlinking;
        if (!(isBlinking||style.Blink)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETBLINK);
        }
        return this;
    }
    /// <summary>
    /// Writes the text inversed or not.
    /// </summary>
    /// <param name="isInversed">Whether the text should be inversed.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Inverse(bool isInversed = true) {
        style.Inverse = isInversed;
        if (!isInversed) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETINVERT);
        }
        return this;
    }
    /// <summary>
    /// Writes the text invisible or not.
    /// </summary>
    /// <param name="isInvisible">Whether the text should be invisible.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Invisible(bool isInvisible = true) {
        style.Invisible = isInvisible;
        if (!isInvisible) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETHIDE);
        }
        return this;
    }
    /// <summary>
    /// Writes the text striketrough or not.
    /// </summary>
    /// <param name="striketrough">Whether the text should be striketrough.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Striketrough(bool striketrough = true) {
        style.Striketrough = striketrough;
        if (!striketrough) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETSTRIKETROUGH);
        }
        return this;
    }
    /// <summary>
    /// Writes the text double underlined or not.
    /// </summary>
    /// <param name="isDoubleUnderlined">Whether the text should be double underlined.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder DoubleUnderline(bool isDoubleUnderlined = true) {
        style.DoubleUnderline = isDoubleUnderlined;
        if (!(isDoubleUnderlined||style.Underline)) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETUNDERLINE);
        }
        return this;
    }
    /// <summary>
    /// Writes the text with a line above or not.
    /// </summary>
    /// <param name="isOverlined">Whether the text should be overlined.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Overline(bool isOverlined = true) {
        style.Overline = isOverlined;
        if (!isOverlined) {
            text += ANSI.SGR.Build(ANSI.SGR.RESETOVERLINE);
        }
        return this;
    }
    /// <summary>
    /// Writes the text with an alternative font to use (0 to 9), null for default. Where 9 is Fraktur, which is rarely supported.
    /// </summary>
    /// <param name="font">Which alternative font to use (0 to 9), null for default. Where 9 is Fraktur, which is rarely supported.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <returns>This style builder.</returns>
    public StyleBuilder Font(byte? font) {
        style.Font = font;
        if (font == null) {
            text += ANSI.SGR.Build(ANSI.SGR.DEFAULTFONT);
        }
        return this;
    }
    /// <summary>
    /// Resets the style of the upcoming text.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder Reset() {
        text += ANSI.SGR.BuildedResetAll;
        style = new();
        return this;
    }
    /// <summary>
    /// Resets the background color of the upcoming text.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder ResetBackground() {
        return Background(StandardColor.Default);
    }
    /// <summary>
    /// Resets the text color of the upcoming text.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder ResetForeground() {
        return Foreground(StandardColor.Default);
    }
    /// <summary>
    /// Resets the underline color of the upcoming text.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder ResetUnderlineColor() {
        return UnderlineColor(PalleteColor.Default);
    }
    /// <summary>
    /// Sets the background color.
    /// </summary>
    /// <param name="backgroundColor">Which background color to use.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Background(IColor backgroundColor) {
        style.BackgroundColor = backgroundColor;
        return this;
    }
    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="foregroundColor">Which text color to use.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder Foreground(IColor foregroundColor) {
        style.ForegroundColor = foregroundColor;
        return this;
    }
    /// <summary>
    /// Sets the underline color.
    /// </summary>
    /// <param name="underlineColor">Which underline color to use.</param>
    /// <returns>This style builder.</returns>
    public StyleBuilder UnderlineColor(ISpecifiedColor underlineColor) {
        style.UnderlineColor = underlineColor;
        return this;
    }

    /// <summary>
    /// Applies all the styles to the text (happens automatically).
    /// </summary>
    public void Apply() {
        text += style.ToANSI(false);
    }
    /// <summary>
    /// Adds text.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder Text(string text) {
        Apply();
        this.text += text;
        return this;
    }
    /// <summary>
    /// Adds a new line.
    /// </summary>
    /// <returns>This style builder.</returns>
    public StyleBuilder NewLine() {
        Apply();
        text += '\n';
        return this;
    }

    /// <summary>
    /// Returns the builded text.
    /// </summary>
    /// <returns>The builded text.</returns>
    public override string ToString() {
        Apply();
        return text;
    }
}