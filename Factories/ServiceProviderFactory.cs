using Microsoft.Extensions.DependencyInjection;
using screenerWpf;
using screenerWpf.Controls;
using screenerWpf.Factories;
using screenerWpf.Interfaces;
using screenerWpf.Sevices;
using System;

public static class ServiceProviderFactory
{
    public static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Main>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();
        services.AddSingleton<IImageEditorWindowFactory, ImageEditorWindowFactory>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ICanvasActionHandler, CanvasActionHandler>();
        services.AddSingleton<ICanvasSelectionHandler, CanvasSelectionHandler>();
        services.AddSingleton<ICanvasEditingHandler, CanvasEditingHandler>();
        services.AddSingleton<ICanvasSavingHandler, CanvasSavingHandler>();
        services.AddSingleton<ICanvasInputHandler, CanvasInputHandler>();
        services.AddSingleton<IOptionsWindowFactory, OptionsWindowFactory>();
        services.AddTransient<ICloudStorageUploader, DropboxUploader>();

        return services.BuildServiceProvider();
    }
}