using System.Net.Http.Headers;

namespace Harmonize.Kodi;

public class KodiClient
{
    private static readonly HttpClient client = new HttpClient();
    private string? _hostName;
    private int? _port;
    private string? _username;
    private string? _password;
    public KodiClient SetPort(int port)
    {
        _port = port;
        return this;
    }
    public KodiClient SetHostName(string hostName)
    {
        _hostName = hostName;
        return this;
    }
    public KodiClient SetUserName(string userName)
    {
        _username = userName;
        return this;
    }
    public KodiClient SetPassword(string password)
    {
        _password = password;
        return this;
    }
    private async Task PostRequestAsync(string requestObject)
    {
        var requestBody = new StringContent(requestObject);
        requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var credential = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_username}:{_password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);

        var request = new HttpRequestMessage(HttpMethod.Post, $"http://{_hostName}:{_port}/jsonrpc");
        request.Content = requestBody;

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonBody = await response.Content.ReadAsStringAsync();
    }

    public async Task PowerOff()
    {
        var requestObject = @"
            {
                ""jsonrpc"": ""2.0"",
                ""method"": ""System.Shutdown"",
                ""id"": 1
            }";

        await PostRequestAsync(requestObject);
    }
    public async Task OpenVideos()
    {
        var requestObject = @"
        {
          ""jsonrpc"": ""2.0"",
          ""id"": 1,
          ""method"": ""GUI.ActivateWindow"",
          ""params"": {
            ""window"": ""videos"",
            ""parameters"": [""/storage/videos""]
          }
        }";

        await PostRequestAsync(requestObject);
    }
    public async Task OpenYoutube()
    {
        var requestObject = @"
        {
          ""jsonrpc"": ""2.0"",
          ""id"": 1,
          ""method"": ""Addons.ExecuteAddon"",
          ""params"": {
            ""addonid"": ""plugin.video.youtube"",
            ""params"": [""action=search""]
          }
        }";

        await PostRequestAsync(requestObject);
    }
    public async Task ShowOSD()
    {
        var requestObject = @"
            {
                ""jsonrpc"": ""2.0"",
                ""method"": ""Input.ShowOSD"",
                ""id"": 1
            }";

        await PostRequestAsync(requestObject);
    }

    public async Task PowerSuspend()
    {
        var requestObject = @"
            {
                ""jsonrpc"": ""2.0"",
                ""method"": ""System.Suspend"",
                ""id"": 1
            }";

        await PostRequestAsync(requestObject);
    }

    public async Task PowerReboot()
    {
        var requestObject = @"
            {
                ""jsonrpc"": ""2.0"",
                ""method"": ""System.Reboot"",
                ""id"": 1
            }";

        await PostRequestAsync(requestObject);
    }

    public async Task SetPlayerSpeed(int speed)
    {
        var requestObject = $@"
            {{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""method"": ""Player.SetSpeed"",
                ""params"": {{
                    ""playerid"": 1,
                    ""speed"": {speed}
                }}
            }}";

        await PostRequestAsync(requestObject);
    }

    public async Task InputSendText(string text)
    {
        var requestObject = $@"
            {{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""method"": ""Input.SendText"",
                ""params"": {{
                    ""text"": ""{text}"",
                    ""done"": true
                }}
            }}";

        await PostRequestAsync(requestObject);
    }

    public async Task InputText(string text)
    {
        var requestObject = $@"
            {{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""method"": ""Input.SendText"",
                ""params"": {{
                    ""text"": ""{text}"",
                    ""done"": false
                }}
            }}";

        await PostRequestAsync(requestObject);
    }
    public async Task SeekPlayerAsync(int percentage)
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Player.Seek"",
        ""params"": {{
            ""playerid"": 1,
            ""value"": {{
                ""percentage"": {percentage}
            }}
        }}
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task SetVolumeAsync(int volumePercentage)
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Application.SetVolume"",
        ""params"": {{
            ""volume"": {volumePercentage}
        }}
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputSelectAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Select""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputBackAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Back""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputExecuteActionAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.ExecuteAction""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputLeftAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Left""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputRightAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Right""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputDownAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Down""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task InputUpAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Input.Up""
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task StopPlayerAsync()
    {
        var requestObject = $@"
    {{
        ""jsonrpc"": ""2.0"",
        ""id"": 1,
        ""method"": ""Player.Stop"",
        ""params"": {{
            ""playerid"": 1
        }}
    }}";
        await PostRequestAsync(requestObject);
    }

    public async Task TogglePlayPausePlayerAsync()
    {
        var requestObject = $@"
        {{
            ""jsonrpc"": ""2.0"",
            ""id"": 1,
            ""method"": ""Player.PlayPause"",
            ""params"": {{
                ""playerid"": 1
            }}
        }}";

        await PostRequestAsync(requestObject);
    }
    public async Task PlayFile(string fileName)
    {
        var requestObject = $@"
        {{
            ""jsonrpc"": ""2.0"",
            ""id"": 1,
            ""method"": ""Player.Open"",
            ""params"": {{
                ""item"": {{
                    ""file"": ""/storage/videos/{fileName}""
                }}
            }}
        }}";

        await PostRequestAsync(requestObject);
    }
    public KodiClient UpdateSettings(string hostname, int port, string username, string password)
    {
        _hostName = hostname;
        _port = port;
        _username = username;
        _password = password;
        return this;
    }
}
