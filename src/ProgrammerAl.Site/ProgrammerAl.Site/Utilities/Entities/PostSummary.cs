using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public class PostSummary
{
    public const string RecentSummariesFile = "RecentSummaries.json";
    public const string AllPostSummariesFile = "AllPostSummaries.json";

    public PostSummary(
        string title,
        DateOnly postedDate,
        string titleLink,
        string firstParagraph,
        int postNumber,
        string[] tags,
        string comicImageLink)
    {
        Title = title;
        PostedDate = postedDate;
        TitleLink = titleLink;
        FirstParagraph = firstParagraph;
        PostNumber = postNumber;
        Tags = tags;
        ComicImageLink = comicImageLink;
    }

    public string Title { get; set; }
    public DateOnly PostedDate { get; set; }
    public string TitleLink { get; set; }
    public string FirstParagraph { get; set; }
    public int PostNumber { get; set; }
    public string[] Tags { get; set; }
    public string ComicImageLink { get; set; }
}
