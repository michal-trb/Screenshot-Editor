using screenerWpf.ViewModels;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Windows.Data;

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

            var appResources = Application.Current.Resources.MergedDictionaries;
            var currentTheme = appResources.FirstOrDefault(rd => rd.Source != null && (rd.Source.OriginalString.Contains("LightStyle") || rd.Source.OriginalString.Contains("DarkStyle")));
            if (currentTheme != null)
            {
                var toggleButton = FindName("ToggleButton") as ToggleButton;
                if (toggleButton != null)
                {
                    toggleButton.IsChecked = currentTheme.Source.OriginalString.Contains("DarkStyle");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as OptionsViewModel;
            viewModel?.SaveSettings();
            this.Close();
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ChangeTheme(dark: true);
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeTheme(dark: false);

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

    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength gridLength = (GridLength)value;
            return gridLength.Value > 0;
        }
    }
}

