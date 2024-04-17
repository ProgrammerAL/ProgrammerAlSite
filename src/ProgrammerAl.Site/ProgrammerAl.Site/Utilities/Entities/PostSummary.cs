using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public class PostSummary
{
    public const string RecentSummariesFile = "RecentSummaries.json";
    public const string AllPostSummariesFile = "AllPostSummaries.json";

    public PostSummary(
        string titleHumanReadable,
        string titleLink,
        DateOnly postedDate,
        string firstParagraph,
        int postNumber,
        string[] tags,
        string comicImageLink)
    {
        TitleHumanReadable = titleHumanReadable;
        TitleLink = titleLink;
        PostedDate = postedDate;
        FirstParagraph = firstParagraph;
        PostNumber = postNumber;
        Tags = tags;
        ComicImageLink = comicImageLink;
    }

    public string TitleHumanReadable { get; set; }
    public string TitleLink { get; set; }
    public DateOnly PostedDate { get; set; }
    public string FirstParagraph { get; set; }
    public int PostNumber { get; set; }
    public string[] Tags { get; set; }
    public string ComicImageLink { get; set; }
}
