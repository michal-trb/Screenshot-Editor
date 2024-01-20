using System;
using System.Windows;

namespace screenerWpf.Helpers
{
    public class ThemeManager
    {
        public event Action ThemeChanged;

        public enum Themes
        {
            Light,
            Dark
        }

        private Themes currentTheme = Themes.Dark;

        public Themes CurrentTheme
        {
            get => currentTheme;
            set
            {
                if (currentTheme != value)
                {
                    currentTheme = value;
                    OnThemeChanged();
                }
            }
        }

        protected virtual void OnThemeChanged()
        {
            ThemeChanged?.Invoke();
        }

        public void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == Themes.Dark ? Themes.Light : Themes.Dark;
            UpdateApplicationTheme();
        }

        private void UpdateApplicationTheme()
        {
            var newTheme = CurrentTheme == Themes.Dark ? "DarkThemeResourceDictionary.xaml" : "LightThemeResourceDictionary.xaml";
            var oldTheme = CurrentTheme == Themes.Dark ? "LightThemeResourceDictionary.xaml" : "DarkThemeResourceDictionary.xaml";

            foreach (ResourceDictionary dict in Application.Current.Resources.MergedDictionaries)
            {
                if (dict.Source != null && dict.Source.OriginalString.Contains(oldTheme))
                {
                    Uri themeUri = new Uri(newTheme, UriKind.Relative);
                    var themeDict = new ResourceDictionary() { Source = themeUri };
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
                    Application.Current.Resources.MergedDictionaries.Add(themeDict);
                    break;
                }
            }
        }
    }

}
