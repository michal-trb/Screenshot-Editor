using Microsoft.Extensions.DependencyInjection;
using screenerWpf;
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
        services.AddSingleton<IImageEditorWindowFactory, ImageEditorWindowFactory>();
        services.AddSingleton<IWindowService, WindowService>();

        return services.BuildServiceProvider();
    }
}