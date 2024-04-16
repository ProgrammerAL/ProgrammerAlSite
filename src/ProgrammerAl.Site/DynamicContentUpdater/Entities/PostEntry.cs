using System;
using System.Collections.Immutable;

namespace DynamicContentUpdater.Entities;

public class PostEntry
{
    public const string ComicsTag = "comic";

    public PostEntry(
        string postDirectoryLocalPath,
        string titleHumanReadable,
        string titleLink,
        DateOnly releaseDate,
        ImmutableArray<string> tags,
        string postMarkdown,
        string postHtml,
        string firstParagraphHtml,
        int postNumber)
    {
        PostDirectoryLocalPath = postDirectoryLocalPath;
        TitleHumanReadable = titleHumanReadable;
        TitleLink = titleLink;
        ReleaseDate = releaseDate;
        Tags = tags;
        PostMarkdown = postMarkdown;
        PostHtml = postHtml;
        PostNumber = postNumber;
        FirstParagraphHtml = firstParagraphHtml;
    }

    public string PostDirectoryLocalPath { get; }
    public string TitleHumanReadable { get; }
    public string TitleLink { get; }
    public DateOnly ReleaseDate { get; }
    public ImmutableArray<string> Tags { get; }
    public string PostMarkdown { get; }
    public string PostHtml { get; }
    public string FirstParagraphHtml { get; }
    public int PostNumber { get; }

    public bool HasComic => !string.IsNullOrWhiteSpace(ComicSvg);

    public bool TryGetComicLink(out string comicLink)
    {
        if (HasComic)
        {
            comicLink = $"{TitleLink}/comic.svg";
            return true;
        }

        comicLink = null;
        return false;
    }
}
