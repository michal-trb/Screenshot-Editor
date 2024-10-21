namespace screenerWpf;

using Microsoft.Extensions.DependencyInjection;
using screenerWpf.Factories;
using screenerWpf.Interfaces;
using screenerWpf.Properties;
using screenerWpf.Sevices;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

/// <summary>
/// Represents the application class for screenerWpf, handling startup, dependency injection, and user interface management.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Provides access to the service provider for dependency injection.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Gets the main view model for the application.
    /// </summary>
    public MainViewModel MainViewModelService { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// Configures the service provider, loads the selected theme, and initializes the main view model.
    /// </summary>
    public App()
    {
        LoadTheme();

        serviceProvider = ServiceProviderFactory.CreateServiceProvider();

        var windowService = serviceProvider.GetRequiredService<IWindowService>() as WindowService;
        var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

        MainViewModelService = mainViewModel;
    }

    /// <summary>
    /// Handles the application startup event, sets up jump lists, and shows the main window.
    /// </summary>
    /// <param name="e">Startup event arguments containing command-line arguments.</param>
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

    /// <summary>
    /// Sets up the jump list for the application, adding various tasks for quick access.
    /// </summary>
    private void SetupJumpList()
    {
        JumpList jumpList = new JumpList();

        // Task for screenshot
        AddJumpTask(jumpList, "Take Screenshot", "Create a new screenshot", "screenshot");

        // Task for area screenshot
        AddJumpTask(jumpList, "Area Screenshot", "Create a screenshot of a selected area", "captureArea");

        // Task for video recording
        AddJumpTask(jumpList, "Record Video", "Start video recording", "recordVideo");

        // Task for area video recording
        AddJumpTask(jumpList, "Area Video Recording", "Record video in a selected area", "recordAreaVideo");

        jumpList.Apply();
        JumpList.SetJumpList(Application.Current, jumpList);
    }

    /// <summary>
    /// Adds a task to the application's jump list.
    /// </summary>
    /// <param name="jumpList">The jump list to which the task will be added.</param>
    /// <param name="title">The title of the task.</param>
    /// <param name="description">The description of the task.</param>
    /// <param name="argument">The command-line argument to pass when the task is executed.</param>
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

    /// <summary>
    /// Handles command-line arguments passed through jump list tasks.
    /// </summary>
    /// <param name="args">The command-line argument to handle.</param>
    private void HandleJumpListArguments(string args)
    {
        switch (args)
        {
            case "screenshot":
                MainViewModelService?.ExecuteCaptureFullAsync();
                break;
            case "captureArea":
                MainViewModelService?.ExecuteCaptureAreaAsync();
                break;
            case "recordVideo":
                MainViewModelService?.ExecuteRecordVideoAsync();
                break;
            case "recordAreaVideo":
                MainViewModelService?.ExecuteAreaRecordVideoAsync();
                break;
            default:
                // Handle unknown arguments or log them
                break;
        }
    }

    /// <summary>
    /// Loads the theme based on user settings and applies it to the application.
    /// </summary>
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
