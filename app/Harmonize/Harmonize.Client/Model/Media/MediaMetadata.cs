namespace Harmonize.Client.Model.Media;
public class MediaMetadata
{
    public required string Title { get; set; }

    public required string Artist { get; set; }

    public required string Album { get; set; }

    public required Artwork Artwork { get; set; }
}

public class Artwork
{
    public required string Xl { get; set; }

    public required string Large { get; set; }

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
