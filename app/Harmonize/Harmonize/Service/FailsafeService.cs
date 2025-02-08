using Microsoft.Extensions.Logging;

namespace Harmonize.Service;

public class FailsafeService(
    AlertService alertService,
    ILogger<FailsafeService> logger
    )
{
    public async Task<(TResult? ResultObject, bool Success)> Fallback<TResult>(
        Func<Task<TResult?>> callback,
        TResult? defaultValueIfFailed = default,
        string? title = null
        )
    {
        try
        {
            TResult? result = await callback();

            if (result == null)
            {
                return (result, false);
            }

            return (result, true);
        }
        catch (Exception ex)
        {
            await alertService.ShowAlertAsync(title ?? "Error", ex.Message ?? "Failure to perform action");

            logger.LogError("Failure: {}", ex);

            return (defaultValueIfFailed, false);
        }
    }
}
