using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public record PostMetadata(string Title, string ComicImageLink, DateOnly ReleaseDate)
{
    public const string FileName = "metadata.json";
}
