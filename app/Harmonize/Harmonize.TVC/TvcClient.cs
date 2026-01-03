namespace Harmonize.TVC;


public enum HdmiInput
{
    One = 1,
    Two = 2,
    Three = 3,
}

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
    public async Task ToggleMute()
    {
        await PostRequestAsync("toggle_mute");
    }
    public async Task VolumeUp()
    {
        await PostRequestAsync("volume_up");
    }
    public async Task VolumeDown()
    {
        await PostRequestAsync("volume_down");
    }
    public async Task HdmiSwitch(HdmiInput hdmiInput)
    {
        await PostRequestAsync($"active_source/{(int)hdmiInput}");
    }
    public TvcClient UpdateSettings(string hostname, int port)
    {
        _hostName = hostname;
        _port = port;
        return this;
    }
}
