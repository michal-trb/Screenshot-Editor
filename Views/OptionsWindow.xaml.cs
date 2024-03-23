using screenerWpf.ViewModels;
using System.Linq;
using System;
using System.Windows;

namespace screenerWpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(OptionsViewModel optionsViewModel)
        {
            InitializeComponent();
            DataContext = optionsViewModel;
            var savedTheme = Properties.Settings.Default.Theme;
            if (!string.IsNullOrEmpty(savedTheme))
            {
                themeToggleButton.IsChecked = savedTheme == "Dark";
            }

            if (!string.IsNullOrEmpty(savedTheme))
            {
                if (savedTheme == "Dark")
                {
                    ChangeTheme("Views\\DarkStyle.xaml");
                }
                else
                {
                    ChangeTheme("Views\\LightStyle.xaml");
                }
            }
        }

        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ChangeTheme("Views\\DarkStyle.xaml");
            Properties.Settings.Default.Theme = "Dark";
            Properties.Settings.Default.Save();
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeTheme("Views\\LightStyle.xaml");
            Properties.Settings.Default.Theme = "Light";
            Properties.Settings.Default.Save();
        }

        private void ChangeTheme(string themePath)
        {
            var newDict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries[1] = newDict;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as OptionsViewModel;
            viewModel?.SaveSettings();
            this.Close();
        }

        private void ChangeTheme(bool dark)
        {
            var appResources = Application.Current.Resources.MergedDictionaries;
            var lightThemePath = @"pack://application:,,,/Views/LightStyle.xaml";
            var darkThemePath = @"pack://application:,,,/Views/DarkStyle.xaml";

            var currentTheme = appResources.FirstOrDefault(rd => rd.Source != null && (rd.Source.OriginalString.Contains("LightStyle") || rd.Source.OriginalString.Contains("DarkStyle")));

            if (dark)
            {
                appResources.Remove(currentTheme);
                appResources.Add(new ResourceDictionary { Source = new Uri(darkThemePath, UriKind.Absolute) });
            }
            else
            {
                appResources.Remove(currentTheme);
                appResources.Add(new ResourceDictionary { Source = new Uri(lightThemePath, UriKind.Absolute) });
            }
        }
    }
}

