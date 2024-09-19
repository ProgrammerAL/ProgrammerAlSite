using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace DynamicContentUpdater.Entities;

public record ParsedEntry(
    string Title,
    DateOnly ReleaseDate,
    ImmutableArray<string> Tags,
    ImmutableArray<string> PresentationSlideUrls,
    string Post,
    string FirstParagraph);
