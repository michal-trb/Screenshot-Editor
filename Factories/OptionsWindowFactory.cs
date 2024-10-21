namespace screenerWpf.Factories;

using screenerWpf.ViewModels;
using screenerWpf.Views;

/// <summary>
/// A factory class responsible for creating instances of <see cref="OptionsWindow"/>.
/// </summary>
public class OptionsWindowFactory : IOptionsWindowFactory
{
    /// <summary>
    /// Creates an instance of <see cref="OptionsWindow"/> with a new <see cref="OptionsViewModel"/>.
    /// </summary>
    /// <returns>An instance of <see cref="OptionsWindow"/>.</returns>
    public OptionsWindow Create()
    {
        return new OptionsWindow(new OptionsViewModel());
    }
}