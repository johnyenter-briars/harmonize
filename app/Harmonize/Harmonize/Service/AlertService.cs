using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Harmonize.Service;


//https://stackoverflow.com/questions/72429055/how-to-displayalert-in-a-net-maui-viewmodel
public class AlertService
{
    readonly Color? BackgroundColor;
    readonly Color? TextColor;
    readonly SnackbarOptions SnackbarOptions = new();
    public AlertService()
    {
        if (Application.Current.Resources.TryGetValue("Gray500", out var colorValue) && colorValue is Color color)
        {
            BackgroundColor = color;
        }

        if (Application.Current.Resources.TryGetValue("Gray950Brush", out var colorValue2) && colorValue2 is Color color2)
        {
            TextColor = color2;
        }

        SnackbarOptions.BackgroundColor = BackgroundColor ?? SnackbarOptions.BackgroundColor;
        SnackbarOptions.TextColor = TextColor ?? SnackbarOptions.TextColor;
    }
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }
    public Task ShowAlertSnackbarAsync(string message, string cancel = "OK", Action? action = null)
    {

        return Application.Current.MainPage.DisplaySnackbar(message, action, cancel, visualOptions: SnackbarOptions);
    }
    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }
    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }
    public void ShowConfirmation(string title, string message, Action<bool> callback,
                                 string accept = "Yes", string cancel = "No")
    {
        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
        {
            bool answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        });
    }
}
