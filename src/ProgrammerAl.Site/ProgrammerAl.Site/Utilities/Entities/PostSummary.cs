using System;

namespace ProgrammerAl.Site.Utilities.Entities;

public class PostSummary
{
    public const string ComicsTag = "comic";

    public PostSummary(
        string title,
        DateOnly postedDate,
        string titleLink,
        string firstParagraph,
        int postNumber,
        string[] tags)
    {
        Title = title;
        PostedDate = postedDate;
        TitleLink = titleLink;
        FirstParagraph = firstParagraph;
        PostNumber = postNumber;
        Tags = tags;

        if (IsComic())
        {
            ComicImageLink = $"{TitleLink}/comic.svg";
        }
    }

    public string Title { get; set; }
    public DateOnly PostedDate { get; set; }
    public string TitleLink { get; set; }
    public string FirstParagraph { get; set; }
    public int PostNumber { get; set; }
    public string[] Tags { get; set; }
    public string ComicImageLink { get; set; }

    public bool IsComic()
        => Tags.Contains(ComicsTag);

    public ComicImageEntry ToComicImageEntry()
        => new ComicImageEntry
        {
            Title = Title,
            ImageLink = ComicImageLink
        };
}
