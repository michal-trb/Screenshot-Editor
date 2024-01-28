using Microsoft.Extensions.DependencyInjection;
using screenerWpf.Interfaces;
using screenerWpf.Properties;
using screenerWpf.Sevices;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

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

            SetupJumpList();

            var mainWindow = serviceProvider.GetRequiredService<Main>();
            mainWindow.DataContext = MainViewModelService;
            mainWindow.Show();

            if (e.Args.Length > 0)
            {
                HandleJumpListArguments(e.Args.FirstOrDefault());
                Shutdown();
                return;
            }
        }

        private void SetupJumpList()
        {
            JumpList jumpList = new JumpList();

            // Task for screenshot
            AddJumpTask(jumpList, "Take Screenshot", "Create a new screenshot", "screenshot");

            // Task for scrolling screenshot
            AddJumpTask(jumpList, "Scrolling Screenshot", "Create a scrolling screenshot", "captureScroll");

            // Task for area screenshot
            AddJumpTask(jumpList, "Area Screenshot", "Create a screenshot of a selected area", "captureArea");

            // Task for video recording
            AddJumpTask(jumpList, "Record Video", "Start video recording", "recordVideo");

            // Task for area video recording
            AddJumpTask(jumpList, "Area Video Recording", "Record video in a selected area", "recordAreaVideo");

            jumpList.Apply();
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        private void AddJumpTask(JumpList jumpList, string title, string description, string argument)
        {
            var fullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            JumpTask task = new JumpTask
            {
                Title = title,
                Description = description,
                ApplicationPath = fullPath,
                Arguments = argument
            };
            jumpList.JumpItems.Add(task);
        }


        private void HandleJumpListArguments(string args)
        {
            switch (args)
            {
                case "screenshot":
                    MainViewModelService?.ExecuteCaptureFull(null);
                    break;
                case "captureScroll":
                    MainViewModelService?.ExecuteCaptureWindowScroll(null);
                    break;
                case "captureArea":
                    MainViewModelService?.ExecuteCaptureArea(null);
                    break;
                case "recordVideo":
                    MainViewModelService?.ExecuteRecordVideo(null);
                    break;
                case "recordAreaVideo":
                    MainViewModelService?.ExecuteAreaRecordVideo(null);
                    break;
                default:
                    // Handle unknown arguments or log them
                    break;
            }
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
