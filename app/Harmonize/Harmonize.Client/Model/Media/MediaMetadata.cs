namespace Harmonize.Client.Model.Media;
public class MediaMetadata
{
    //[JsonProperty("title")]
    public required string Title { get; set; }

    //[JsonProperty("artist")]
    public required string Artist { get; set; }

    //[JsonProperty("album")]
    public required string Album { get; set; }

    //[JsonProperty("artwork")]
    public required Artwork Artwork { get; set; }
}

public class Artwork
{
    //[JsonProperty("xl")]
    public required string Xl { get; set; }

    //[JsonProperty("large")]
    public required string Large { get; set; }

    //[JsonProperty("small")]
    public required string Small { get; set; }
}


//public partial class MediaMetadata
//{
//    public static MediaMetadata FromJson(string json) => JsonConvert.DeserializeObject<MediaMetadata>(json, Converter.Settings);
//}

//public static class Serialize
//{
//    public static string ToJson(this MediaMetadata self) => JsonConvert.SerializeObject(self, Converter.Settings);
//}
