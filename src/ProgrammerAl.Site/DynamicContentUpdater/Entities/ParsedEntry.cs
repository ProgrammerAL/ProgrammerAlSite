using System;
using System.Collections.Immutable;

using static DynamicContentUpdater.Entities.ParsedEntry;

namespace DynamicContentUpdater.Entities;

public record ParsedEntry(
    string Title,
    DateOnly ReleaseDate,
    ImmutableArray<string> Tags,
    ImmutableArray<PresentationEntry> Presentations,
    string Post,
    string FirstParagraph)
{
    public record PresentationEntry(int Id, string SlidesRootUrl);
}
