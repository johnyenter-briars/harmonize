namespace Harmonize.TVC;

public class TvcClient
{
    private static readonly HttpClient client = new HttpClient();
    private string? _hostName;
    private int? _port;
    public TvcClient SetPort(int port)
    {
        _port = port;
        return this;
    }
    public TvcClient SetHostName(string hostName)
    {
        _hostName = hostName;
        return this;
    }
    private async Task PostRequestAsync(string path)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"http://{_hostName}:{_port}/api/{path}");

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var _ = await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            return;
        }
    }
    public async Task PowerOff()
    {
        await PostRequestAsync("power_off");
    }
    public async Task PowerOn()
    {
        await PostRequestAsync("power_on");
    }
    public TvcClient UpdateSettings(string hostname, int port)
    {
        _hostName = hostname;
        _port = port;
        return this;
    }
}
