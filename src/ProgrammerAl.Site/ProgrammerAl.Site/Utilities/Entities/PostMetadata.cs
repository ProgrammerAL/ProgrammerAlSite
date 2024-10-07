using System;

using static ProgrammerAl.Site.Utilities.Entities.PostMetadata;

namespace ProgrammerAl.Site.Utilities.Entities;

public record PostMetadata(
    string Title,
    string? ComicImageLink,
    DateOnly ReleaseDate,
    ImmutableArray<PresentationData> Presentations)
{
    public const string FileName = "metadata.json";

    public record PresentationData(int Id, string SlidesUrl, string? SlidesImagesUrl);
}
