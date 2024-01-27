using Microsoft.Extensions.DependencyInjection;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using System;
using System.Windows;
using screenerWpf.Properties;

namespace screenerWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;
        public MainViewModel MainViewModelService { get; private set; }

        public App()
        {
            LoadTheme();

            serviceProvider = ServiceProviderFactory.CreateServiceProvider();

            var windowService = serviceProvider.GetRequiredService<IWindowService>() as WindowService;
            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

            if (windowService != null)
            {
                windowService.MainViewModel = mainViewModel;
            }

            MainViewModelService = mainViewModel;

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = serviceProvider.GetRequiredService<Main>();
            // Ustaw DataContext głównego okna
            mainWindow.DataContext = MainViewModelService;
            mainWindow.Show();
        }

        private void LoadTheme()
        {
            string savedTheme = Settings.Default.Theme;
            string themePath;

            switch (savedTheme)
            {
                case "Dark":
                    themePath = "Views/DarkStyle.xaml";
                    break;
                default:
                    themePath = "Views/LightStyle.xaml";
                    break;
            }

            var themeResourceDictionary = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
            var sourceResourceDictionary = new ResourceDictionary { Source = new Uri("Views\\Styles.xaml", UriKind.Relative) };

            Application.Current.Resources.MergedDictionaries.Add(sourceResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(themeResourceDictionary);
        }

    }
}
