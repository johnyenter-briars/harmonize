using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace Harmonize;

public partial class App : Application
{
    private readonly ILogger<App> logger;

    public App(
        ILogger<App> logger,
        PreferenceManager preferenceManager
        )
    {
        InitializeComponent();

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

        MainPage = new AppShell(preferenceManager);
        this.logger = logger;
    }
    //https://stackoverflow.com/questions/72455467/how-do-you-create-a-global-error-handling-for-a-maui-app
    private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
         logger.LogError($"***** Handling Unhandled Exception *****: {e.Exception.Message}");
    }
}
