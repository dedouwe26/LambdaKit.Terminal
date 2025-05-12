using System.Numerics;

namespace OxDED.Terminal;

/// <summary>
/// Represents a color for a terminal.
/// </summary>
public interface IColor : IEquatable<IColor>, ICloneable {
    /// <summary>
    /// Generates a partial foreground SGR code.
    /// </summary>
    /// <returns>A partial escape code.</returns>
    public string ToForegroundSGR();
    /// <summary>
    /// Generates a partial foreground SGR code.
    /// </summary>
    /// <returns>A partial escape code.</returns>
    public string ToBackgroundSGR();
    /// <summary>
    /// Creates an ANSI string for the background color.
    /// </summary>
    /// <returns>The ANSI string.</returns>
    public string ToBackgroundANSI();
    
    /// <summary>
    /// Creates an ANSI string for the foreground color.
    /// </summary>
    /// <returns>The ANSI string.</returns>
    public string ToForegroundANSI();

    /// <summary>
    /// Clones this color.
    /// </summary>
    /// <returns>A new matching color.</returns>
    public IColor CloneColor();
}
/// <summary>
/// Represents a specified color (8-bit and 24-bit). 
/// </summary>
public interface ISpecifiedColor : IColor {
    /// <summary>
    /// Generates a partial underline SGR code.
    /// </summary>
    /// <returns>A partial escape code.</returns>
    public string ToUnderlineSGR();
    /// <summary>
    /// Creates an ANSI string for the underline color.
    /// </summary>
    /// <returns>The ANSI string.</returns>
    public string ToUnderlineANSI();
}

/// <summary>
/// Represents a standard terminal-defined color (3-bit and 4-bit colors).
/// </summary>
public class StandardColor : IColor {
    /// <summary>
    /// These are standard terminal-defined colors (3-bit and 4-bit colors).
    /// </summary>
    public enum Colors : byte {
        ///
        Black = 30,
        ///
        Red = 31,
        ///
        Green = 32,
        ///
        Yellow = 33,
        ///
        Blue = 34,
        ///
        Magenta = 35,
        ///
        Cyan = 36,
        ///
        White = 37,
        
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightBlack = 90,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightRed = 91,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightGreen = 92,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightYellow = 93,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightBlue = 94,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightMagenta = 95,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightCyan = 96,
        /// <summary>
        /// Only in terminals that support the aixterm specification.
        /// </summary>
        BrightWhite = 97,

        /// <summary>
        /// Resets the color to default.
        /// </summary>
        Default = 39,
    }

    /// <summary>
    /// Creates a new standard color.
    /// </summary>
    /// <param name="color">The terminal-defined color.</param>
    public StandardColor(Colors color) {
        this.color = color;
    }
    /// <summary>
    /// The terminal-defined color.
    /// </summary>
    public Colors color;

    /// <inheritdoc/>
    public string ToForegroundSGR() {
        return ((byte)color).ToString();
    }
    /// <inheritdoc/>
    public string ToBackgroundSGR() {
        return ((byte)color+10).ToString();
    }
    /// <inheritdoc/>
    public string ToBackgroundANSI() {
        return ANSI.SGR.Build(ToBackgroundSGR());
    }
    /// <inheritdoc/>
    public string ToForegroundANSI() {
        return ANSI.SGR.Build(ToForegroundSGR());
    }

	/// <inheritdoc/>
	public override bool Equals(object? obj) {
		return Equals(obj as IColor);
	}
    /// <inheritdoc/>
	public override int GetHashCode() => color.GetHashCode();
    /// <inheritdoc/>
    public bool Equals(IColor? other) {
        if (other == null) return false;
        if (other is not StandardColor standardColor) return false;

        return standardColor.color == color;
    }
    /// <inheritdoc/>
    public object Clone() {
        return CloneColor();
    }
    /// <summary>
    /// Clones this color.
    /// </summary>
    /// <returns>A new matching color.</returns>
    public IColor CloneColor() {
        return new StandardColor(color);
    }

    /// <summary>
    /// The default color of the terminal.
    /// </summary>
    public static readonly StandardColor Default = (StandardColor)Colors.Default;

    /// 
    public static implicit operator StandardColor(Colors color) { return new(color); }
    /// 
    public static explicit operator Colors(StandardColor color) {  return color.color; }
}
/// <summary>
/// Represents a specified color (8-bit colors).
/// </summary>
public class PalleteColor : ISpecifiedColor {
    /// <summary>
    /// Creates a new specified color.
    /// </summary>
    /// <param name="color">The terminal-generated color, null for default.</param>
    public PalleteColor(byte? color) {
        this.color = color;
    }
    /// <summary>
    /// The terminal-generated color.
    /// </summary>
    public byte? color;

    /// <inheritdoc/>
    public string ToForegroundSGR() {
        return color == null ? ANSI.SGR.DEFAULTFOREGROUND : ANSI.SGR.SpecifiedForeground(color.Value);
    }
    /// <inheritdoc/>
    public string ToBackgroundSGR() {
        return color == null ? ANSI.SGR.DEFAULTBACKGROUND : ANSI.SGR.SpecifiedBackground(color.Value);
    }
    /// <inheritdoc/>
    public string ToBackgroundANSI() {
        return ANSI.SGR.Build(ToBackgroundSGR());
    }
    /// <inheritdoc/>
    public string ToForegroundANSI() {
        return ANSI.SGR.Build(ToForegroundSGR());
    }
    /// <summary>
    /// The default color of the terminal, does support underline default as opposed to <see cref="StandardColor.Default"/>.
    /// </summary>
    public static readonly PalleteColor Default = new(null);
    /// <inheritdoc/>
    public string ToUnderlineSGR() {
        return color == null ? ANSI.SGR.DEFAULTUNDERLINE : ANSI.SGR.SpecifiedUnderline(color.Value);
    }
    /// <inheritdoc/>
    public string ToUnderlineANSI() {
        return ANSI.SGR.Build(ToUnderlineSGR());
    }

	/// <inheritdoc/>
	public override bool Equals(object? obj) {
		return Equals(obj as IColor);
	}
    /// <inheritdoc/>
	public override int GetHashCode() => color.GetHashCode();
    /// <inheritdoc/>
    public bool Equals(IColor? other) {
        if (other == null) return false;
        if (other is not PalleteColor specifiedColor) return false;

        return specifiedColor.color == color;
    }
    /// <inheritdoc/>
    public object Clone() {
        return CloneColor();
    }
    /// <summary>
    /// Clones this color.
    /// </summary>
    /// <returns>A new matching color.</returns>
    public IColor CloneColor() {
        return new PalleteColor(color);
    }
}
/// <summary>
/// Represents a specified color with RGB (24-bit colors).
/// </summary>
public class RGBColor : ISpecifiedColor {
    /// <summary>
    /// Creates a new rgb color.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    public RGBColor(byte r, byte g, byte b) {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    /// <summary>
    /// Creates a terminal color from a hex code (true color).
    /// </summary>
    /// <param name="hex">The hex code without the (#), must be 6 long.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public RGBColor(string hex) {
        ArgumentOutOfRangeException.ThrowIfNotEqual(hex.Length, 6, nameof(hex));
        r = Convert.ToByte(hex[..2], 16);
        g = Convert.ToByte(hex.Substring(2, 2), 16);
        b = Convert.ToByte(hex.Substring(4, 2), 16);
    }
    /// <summary>
    /// The red value.
    /// </summary>
    public byte r;
    /// <summary>
    /// The green value.
    /// </summary>
    public byte g;
    /// <summary>
    /// The blue value.
    /// </summary>
    public byte b;

    /// <summary>
    /// Creates a hex string from this color.
    /// </summary>
    /// <returns>The generated hex code. The format is RRGGBB, where R/G/B = 0-F.</returns>-
    public string ToHex() {
        return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }
    /// <inheritdoc/>
    public string ToForegroundSGR() {
        return ANSI.SGR.SpecifiedForeground(r, g, b);
    }
    /// <inheritdoc/>
    public string ToBackgroundSGR() {
        return ANSI.SGR.SpecifiedBackground(r, g, b);
    }
    /// <inheritdoc/>
    public string ToBackgroundANSI() {
        return ANSI.SGR.Build(ToBackgroundSGR());
    }
    /// <inheritdoc/>
    public string ToForegroundANSI() {
        return ANSI.SGR.Build(ToForegroundSGR());
    }
    /// <inheritdoc/>
    public string ToUnderlineSGR() {
        return ANSI.SGR.SpecifiedUnderline(r, g, b);
    }
    /// <inheritdoc/>
    public string ToUnderlineANSI() {
        return ANSI.SGR.Build(ToUnderlineSGR());
    }

	/// <inheritdoc/>
	public override bool Equals(object? obj) {
		return Equals(obj as IColor);
	}
    /// <inheritdoc/>
	public override int GetHashCode() {
		return r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();
	}
    /// <inheritdoc/>
    public bool Equals(IColor? other) {
        if (other == null) return false;
        if (other is not RGBColor rgb) return false;

        return rgb.r == r && rgb.g == g && rgb.b == b;
    }
    /// <inheritdoc/>
    public object Clone() {
        return CloneColor();
    }
    /// <summary>
    /// Clones this color.
    /// </summary>
    /// <returns>A new matching color.</returns>
    public IColor CloneColor() {
        return new RGBColor(r, g, b);
    }

    /// <summary>
    /// 255, 0, 0
    /// </summary>
    public static readonly RGBColor Red = new(255, 0, 0);
    /// <summary>
    /// 0, 255, 0
    /// </summary>
    public static readonly RGBColor Green = new(0, 255, 0);
    /// <summary>
    /// 0, 0, 255
    /// </summary>
    public static readonly RGBColor Blue = new(0, 0, 255);
    /// <summary>
    /// 255, 80, 80
    /// </summary>
    public static readonly RGBColor LightRed = new(255, 80, 80);
    /// <summary>
    /// 30, 190, 30
    /// </summary>
    public static readonly RGBColor DarkGreen = new(30, 190, 30);
    /// <summary>
    /// 30, 30, 190
    /// </summary>
    public static readonly RGBColor DarkBlue = new(30, 30, 190);
    /// <summary>
    /// 255, 255, 0
    /// </summary>
    public static readonly RGBColor Yellow = new(255, 255, 0);
    /// <summary>
    /// 255, 0, 255
    /// </summary>
    public static readonly RGBColor Magenta = new(255, 0, 255);
    /// <summary>
    /// 0, 255, 255
    /// </summary>
    public static readonly RGBColor Cyan = new(0, 255, 255);
    /// <summary>
    /// 255, 160, 0
    /// </summary>
    public static readonly RGBColor Orange = new(255, 160, 0);
    /// <summary>
    /// 180, 180, 180
    /// </summary>
    public static readonly RGBColor Gray = new(180, 180, 180);
    /// <summary>
    /// 64, 64, 64
    /// </summary>
    public static readonly RGBColor DarkGray = new(64, 64, 64);
    /// <summary>
    /// 0, 0, 0
    /// </summary>
    public static readonly RGBColor Black = new(0, 0, 0);
    /// <summary>
    /// 255, 255, 255
    /// </summary>
    public static readonly RGBColor White = new(255, 255, 255);
}