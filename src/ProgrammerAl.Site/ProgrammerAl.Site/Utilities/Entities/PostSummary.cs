using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public record PostSummary(
    string TitleHumanReadable,
    string TitleLink,
    DateOnly PostedDate,
    string FirstParagraph,
    int PostNumber,
    string[] Tags,
    string? ComicImageLink)
{
    public const string RecentSummariesFile = "RecentSummaries.json";
    public const string AllPostSummariesFile = "AllPostSummaries.json";
    public const string AllDraftSummariesFile = "AllDraftSummaries.json";
}
