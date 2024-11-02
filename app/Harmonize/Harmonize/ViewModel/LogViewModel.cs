using Harmonize.Service;
using Microsoft.Extensions.Logging;
using static Harmonize.Constants;

namespace Harmonize.ViewModel;

public class LogViewModel(
    ILogger<LogViewModel> logger,
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private string? logText;
    public string? LogText
    {
        get { return logText; }
        set { SetProperty(ref logText, value); }
    }

    async Task SetLogText()
    {
        if (File.Exists(LogFilePath))
        {
            var logEntries = await File.ReadAllLinesAsync(LogFilePath);

            Array.Reverse(logEntries);

            LogText = string.Join(Environment.NewLine, logEntries);
        }
    }

    public override async Task OnAppearingAsync()
    {
        await SetLogText();
    }
}
