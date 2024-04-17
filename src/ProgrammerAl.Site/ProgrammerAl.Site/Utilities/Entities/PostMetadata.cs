using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public class PostMetadata
{
    public const string FileName = "metadata.json";

    public PostMetadata(
        string title,
        string comicImageLink)
    {
        Title = title;
        ComicImageLink = comicImageLink;
    }

    public string Title { get; set; }
    public string ComicImageLink { get; set; }
}
