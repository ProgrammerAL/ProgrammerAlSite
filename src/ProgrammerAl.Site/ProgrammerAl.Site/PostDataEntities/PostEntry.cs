using System;
using System.Collections.Immutable;
using System.IO;

using static ProgrammerAl.Site.PostDataEntities.PostEntry;

namespace ProgrammerAl.Site.PostDataEntities;

public record PostEntry(
        string PostDirectoryLocalPath,
        string TitleHumanReadable,
        string TitleLink,
        DateOnly ReleaseDate,
        ImmutableArray<string> Tags,
        ImmutableArray<PresentationEntry> Presentations,
        string PostMarkdown,
        string PostHtml,
        string FirstParagraphHtml,
        int PostNumber,
        bool IsDraft)
{
    public const string ComicsTag = "comic";
    public const string ComicSvgFileName = "comic.svg";
    public const string ComicPngFileName = "comic.png";
    public const string HtmlFileName = "post.html";
    public const string MetaTagsFileName = "metatags.html";

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

    public record PresentationEntry(int Id, string SlidesUrl, string SlideImagesUrl);
}
