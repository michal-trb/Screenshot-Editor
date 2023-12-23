using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

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
            serviceProvider = ServiceProviderFactory.CreateServiceProvider();
            MainViewModelService = serviceProvider.GetRequiredService<MainViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = serviceProvider.GetRequiredService<Main>();
            // Ustaw DataContext głównego okna
            mainWindow.DataContext = MainViewModelService;
            mainWindow.Show();
        }

    }
}
