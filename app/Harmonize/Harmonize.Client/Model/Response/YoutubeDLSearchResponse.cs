using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Client.Model.Response;

internal class YoutubeDLSearchResponse
{
    public List<YoutubeDLSearchResult> Results { get; set; }
}
public class YoutubeDLSearchResult
{
    public string Type { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public string PublishedTime { get; set; }
    public string Duration { get; set; }
    public ViewCount ViewCount { get; set; }
    public List<Thumbnail> Thumbnails { get; set; }
    public Thumbnail RichThumbnail { get; set; }
    public List<DescriptionSnippet> DescriptionSnippet { get; set; }
    public Channel Channel { get; set; }
    public Accessibility Accessibility { get; set; }
    public string Link { get; set; }
    public string ShelfTitle { get; set; }
}

public class ViewCount
{
    public string Text { get; set; }
    public string Short { get; set; }
}

public class Thumbnail
{
    public string Url { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class DescriptionSnippet
{
    public string Text { get; set; }
    public bool Bold { get; set; }
}

public class Channel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public List<Thumbnail> Thumbnails { get; set; }
    public string Link { get; set; }
}

public class Accessibility
{
    public string Title { get; set; }
    public string Duration { get; set; }
}
