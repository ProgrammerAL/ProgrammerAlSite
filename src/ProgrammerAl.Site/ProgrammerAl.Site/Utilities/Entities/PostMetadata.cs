using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public record PostMetadata(
    string Title,
    string ComicImageLink,
    DateOnly ReleaseDate,
    ImmutableArray<string> PresentationSlideUrls)
{
    public const string FileName = "metadata.json";
}
