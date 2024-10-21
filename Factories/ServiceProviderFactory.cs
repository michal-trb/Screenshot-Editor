namespace screenerWpf.Factories;

using Microsoft.Extensions.DependencyInjection;
using screenerWpf;
using screenerWpf.Controls;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using System;

public static class ServiceProviderFactory
{
    /// <summary>
    /// Creates and configures an <see cref="IServiceProvider"/> with all required services.
    /// </summary>
    /// <returns>A configured <see cref="IServiceProvider"/> containing all necessary services for the application.</returns>
    public static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        // Register main window and its ViewModel as singleton services
        services.AddSingleton<Main>();
        services.AddSingleton<MainViewModel>();

        // Register core services
        services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();
        services.AddSingleton<IImageEditorControlFactory, ImageEditorControlFactory>();
        services.AddSingleton<IWindowService, WindowService>();

        // Register handlers for canvas-related actions as singleton services
        services.AddSingleton<ICanvasActionHandler, CanvasActionHandler>();
        services.AddSingleton<ICanvasSelectionHandler, CanvasSelectionHandler>();
        services.AddSingleton<ICanvasEditingHandler, CanvasEditingHandler>();
        services.AddSingleton<ICanvasSavingHandler, CanvasSavingHandler>();
        services.AddSingleton<ICanvasInputHandler, CanvasInputHandler>();

        // Register factory for creating options window
        services.AddSingleton<IOptionsWindowFactory, OptionsWindowFactory>();

        return services.BuildServiceProvider();
    }
}
