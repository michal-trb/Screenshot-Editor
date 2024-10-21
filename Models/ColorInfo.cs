namespace screenerWpf.Models;

using System.Windows.Media;

/// <summary>
/// Represents a color information object that holds a color brush.
/// </summary>
public class ColorInfo
{
    /// <summary>
    /// Gets or sets the color brush used for rendering.
    /// </summary>
    public required SolidColorBrush ColorBrush { get; set; }
}
