namespace Harmonize.Extensions;

static class TaskExtensions
{
    public static void FireAndForget(this Task task, Action<Exception> onException = null)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
        });
    }
}
