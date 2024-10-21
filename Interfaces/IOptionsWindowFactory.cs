namespace screenerWpf.Factories;

using screenerWpf.Views;

/// <summary>
/// Interface for creating instances of <see cref="OptionsWindow"/>.
/// </summary>
public interface IOptionsWindowFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="OptionsWindow"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="OptionsWindow"/>.</returns>
    OptionsWindow Create();
}
