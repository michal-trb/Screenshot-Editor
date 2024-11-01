namespace screenerWpf.Views;

using screenerWpf.ViewModels;
using System;
using System.Windows;

/// <summary>
/// Interaction logic for OptionsWindow.xaml.
/// This window allows users to configure application settings, such as changing themes and saving preferences.
/// </summary>
public partial class OptionsWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsWindow"/> class.
    /// Sets the initial theme and binds the provided ViewModel.
    /// </summary>
    /// <param name="optionsViewModel">ViewModel to bind data to the view.</param>
    public OptionsWindow(OptionsViewModel optionsViewModel)
    {
        InitializeComponent();
        DataContext = optionsViewModel;

        var savedTheme = Properties.Settings.Default.Theme;
        if (!string.IsNullOrEmpty(savedTheme))
        {
            ChangeTheme(savedTheme == "Dark" ? "Views\\DarkStyle.xaml" : "Views\\LightStyle.xaml");
        }
    }

    /// <summary>
    /// Changes the theme of the application by loading the specified resource dictionary.
    /// </summary>
    /// <param name="themePath">The path to the theme's resource dictionary.</param>
    private void ChangeTheme(string themePath)
    {
        var newDict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
        Application.Current.Resources.MergedDictionaries[1] = newDict;
    }

    /// <summary>
    /// Saves the current settings when the save button is clicked.
    /// </summary>
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is OptionsViewModel viewModel)
        {
            viewModel.SaveSettings();
            this.Close();
        }
    }
}
