using System.Configuration;
using System.Data;
using System.Windows;

namespace ProjectOrganizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    ShutdownMode = ShutdownMode.OnLastWindowClose; // Keep app alive until all windows close
}
}

