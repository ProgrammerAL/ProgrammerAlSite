using System;
using System.Collections.Immutable;
using System.IO;

namespace ProgrammerAl.Site.PostDataEntities;

public class PostEntry
{
    public const string ComicsTag = "comic";
    public const string ComicSvgFileName = "comic.svg";
    public const string ComicPngFileName = "comic.png";
    public const string HtmlFileName = "post.html";
    public const string MetaTagsFileName = "metatags.html";

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

    public bool HasComic => File.Exists($"{PostDirectoryLocalPath}/{ComicSvgFileName}");

    public bool TryGetComicSvgLink(out string? comicLink)
    {
        if (HasComic)
        {
            comicLink = $"{TitleLink}/{ComicSvgFileName}";
            return true;
        }

        comicLink = null;
        return false;
    }

    public bool TryGetComicPngLink(out string? comicLink)
    {
        if (HasComic)
        {
            comicLink = $"{TitleLink}/{ComicPngFileName}";
            return true;
        }

        comicLink = null;
        return false;
    }
}
