using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace MatVisor;

public partial class App : Application
{
    internal AppLocalizer Localizer { get; }

    public App()
    {
        Localizer = new AppLocalizer();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow(desktop.Args ?? []);
            desktop.MainWindow = mainWindow;

            if (TryGetFeature(typeof(IActivatableLifetime)) is IActivatableLifetime activatableLifetime)
            {
                activatableLifetime.Activated += async (_, activation) =>
                {
                    if (activation is not FileActivatedEventArgs { Kind: ActivationKind.File } fileActivation)
                    {
                        return;
                    }

                    var matFile = fileActivation.Files.FirstOrDefault(file =>
                        string.Equals(Path.GetExtension(file.Name), ".mat", StringComparison.OrdinalIgnoreCase));

                    if (matFile is not null)
                    {
                        await mainWindow.OpenFileAsync(matFile.Path.LocalPath);
                        mainWindow.Activate();
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
