using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Service;

public class FailsafeService(
    AlertService alertService,
    ILogger<FailsafeService> logger
    )
{
    public async Task<(TResult ResultObject, bool Success)> Fallback<TResult>(
        Func<Task<TResult>> callback,
        TResult defaultValueIfFailed,
        string? message = null,
        string? title = null
        )
    {
        try
        {
            TResult result = await callback();

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
