using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MusicCollection.Views;

namespace MusicCollection;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var sp = Services.BuildServiceProvider();
            desktop.MainWindow = new MainWindow(sp);
        }
        base.OnFrameworkInitializationCompleted();
    }
}