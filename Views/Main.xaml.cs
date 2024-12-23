namespace screenerWpf;

using screenerWpf.Factories;
using screenerWpf.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

public partial class Main : Window
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private IntPtr _windowHandle;
    private const int HOTKEY_ID = 9000;
    private HwndSource _source;
    private readonly IOptionsWindowFactory optionsWindowFactory;
    MainViewModel viewModel;

    /// <summary>
    /// Constructor for the Main Window.
    /// Initializes the main window and sets up hotkeys and data context.
    /// </summary>
    /// <param name="viewModel">The ViewModel for the main window.</param>
    /// <param name="optionsWindowFactory">Factory to create options window.</param>
    public Main(MainViewModel viewModel, IOptionsWindowFactory optionsWindowFactory)
    {
        InitializeComponent();
        InitializeHotKey();

        this.optionsWindowFactory = optionsWindowFactory;
        this.viewModel = viewModel;
        DataContext = viewModel;

        this.SizeChanged += Main_SizeChanged;
    }

    /// <summary>
    /// Initializes and registers hotkeys for the application.
    /// </summary>
    private void InitializeHotKey()
    {
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Loaded += (s, e) =>
            {
                var helper = new WindowInteropHelper(Application.Current.MainWindow);
                _windowHandle = helper.Handle;
                _source = HwndSource.FromHwnd(_windowHandle);
                _source.AddHook(HwndHook);

                // Register Print Screen key (0x2C) without modifiers
                RegisterHotKey(_windowHandle, HOTKEY_ID, 0, 0x2C);
                // Register Shift + Print Screen
                RegisterHotKey(_windowHandle, HOTKEY_ID + 1, 0x0004, 0x2C);
            };

            Application.Current.MainWindow.Closing += (s, e) =>
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
                UnregisterHotKey(_windowHandle, HOTKEY_ID + 1);
                _source.RemoveHook(HwndHook);
            };
        }
    }

    /// <summary>
    /// Handles the registered hotkeys.
    /// </summary>
    /// <param name="hwnd">Window handle.</param>
    /// <param name="msg">Message identifier.</param>
    /// <param name="wParam">Additional message information.</param>
    /// <param name="lParam">Additional message information.</param>
    /// <param name="handled">Indicates whether the message is handled.</param>
    /// <returns>Returns IntPtr.Zero.</returns>
    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;
        if (msg == WM_HOTKEY)
        {
            switch (wParam.ToInt32())
            {
                case HOTKEY_ID:
                    Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureAreaAsync());
                    handled = true;
                    break;
                case HOTKEY_ID + 1:
                    Application.Current.Dispatcher.Invoke(() => viewModel.ExecuteCaptureFullAsync());
                    handled = true;
                    break;
            }
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// Opens the options window.
    /// </summary>
    private void OptionsButton_Click(object sender, RoutedEventArgs e)
    {
        var optionsWindow = optionsWindowFactory.Create();
        optionsWindow.ShowDialog();
    }

    /// <summary>
    /// Minimizes the current window.
    /// </summary>
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Maximizes or restores the current window.
    /// </summary>
    private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// Closes the current window.
    /// </summary>
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Allows the user to drag the window by clicking on the title bar.
    /// </summary>
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    /// <summary>
    /// Opens the screenshot associated with the clicked image.
    /// </summary>
    private void Screenshot_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image image && image.DataContext is LastScreenshot screenshot)
        {
            (DataContext as MainViewModel)?.OpenScreenshot(screenshot.FilePath);
        }
    }

    /// <summary>
    /// Opens the video associated with the clicked image.
    /// </summary>
    private void Video_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image image && image.DataContext is LastVideo video)
        {
            (DataContext as MainViewModel)?.OpenVideo(video.FilePath);
        }
    }

    /// <summary>
    /// Expands the height of the window when an Expander is expanded.
    /// </summary>
    private void Expander_Expanded(object sender, RoutedEventArgs e)
    {
        this.Height += 105;
        UpdateWindowWidth();
    }

    /// <summary>
    /// Collapses the height of the window when an Expander is collapsed.
    /// </summary>
    private void Expander_Collapsed(object sender, RoutedEventArgs e)
    {
        this.Height -= 105;
        UpdateWindowWidth();
    }

    /// <summary>
    /// Updates the window width depending on whether any Expander is expanded.
    /// </summary>
    private void UpdateWindowWidth()
    {
            this.Width = 304; // Decrease width when all Expanders are collapsed
    }

    /// <summary>
    /// Opens the folder containing screenshots.
    /// </summary>
    private void OpenScreenshotsFolder(object sender, RoutedEventArgs e)
    {
        string folderPath = Properties.Settings.Default.ScreenshotsLibrary;
        if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
        {
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
        else
        {
            MessageBox.Show("Folder path is not set or does not exist.", "Error");
        }
    }

    /// <summary>
    /// Opens the folder containing recorded videos.
    /// </summary>
    private void OpenVideosFolder(object sender, RoutedEventArgs e)
    {
        string folderPath = Properties.Settings.Default.RecordsSavePath;
        if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
        {
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
        else
        {
            MessageBox.Show("Folder path is not set or does not exist.", "Error");
        }
    }

    /// <summary>
    /// Displays the screenshot editor for the given screenshot.
    /// </summary>
    /// <param name="screenshot">BitmapSource to be edited.</param>
    public void DisplayScreenshotEditor(BitmapSource screenshot)
    {
        var screenshotEditor = new ImageEditorControl(screenshot);
        screenshotEditor.LoadedAndSizeUpdated += ScreenshotEditor_LoadedAndSizeUpdated;
        ScreenshotEditorGrid.Children.Clear();
        ScreenshotEditorGrid.Children.Add(screenshotEditor);
    }

    /// <summary>
    /// Adjusts the window size based on the loaded screenshot editor size.
    /// </summary>
    private void ScreenshotEditor_LoadedAndSizeUpdated(ImageEditorControl element)
    {
        AdjustWindowSize(element);
    }

    /// <summary>
    /// Adjusts the window size to accommodate the given element.
    /// </summary>
    /// <param name="element">The ImageEditorControl element used to determine the new size.</param>
    private void AdjustWindowSize(ImageEditorControl element)
    {
        this.Width = element.Width + 40;
        this.Height = element.Height + 40;
    }

    private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ScreenshotEditorGrid.Children.Count > 0 && ScreenshotEditorGrid.Children[0] is ImageEditorControl editor)
        {
            double minWidth = 920;
            double minHeight = 600;

            double newWidth = Math.Max(e.NewSize.Width - 40, minWidth);
            double newHeight = Math.Max(e.NewSize.Height - 40, minHeight);

            editor.Width = newWidth;
            editor.Height = newHeight;
        }
    }
}