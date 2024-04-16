using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace DynamicContentUpdater.Entities;

public class PostEntry
{
    public const string ComicsTag = "comic";

    public PostEntry(
        string titleHumanReadable,
        string titleLink,
        DateOnly releaseDate,
        ImmutableArray<string> tags,
        string postMarkdown,
        string postHtml,
        string firstParagraphHtml,
        int postNumber)
    {
        TitleHumanReadable = titleHumanReadable;
        TitleLink = titleLink;
        ReleaseDate = releaseDate;
        Tags = tags;
        PostMarkdown = postMarkdown;
        PostHtml = postHtml;
        PostNumber = postNumber;
        FirstParagraphHtml = firstParagraphHtml;
    }

    public string TitleHumanReadable { get; }
    public string TitleLink { get; }
    public DateOnly ReleaseDate { get; }
    public ImmutableArray<string> Tags { get; }
    public string PostMarkdown { get; }
    public string PostHtml { get; }
    public string FirstParagraphHtml { get; }
    public int PostNumber { get; }

    public bool TryGetComicLink(out string comicLink)
    {
        if (Tags.Contains(ComicsTag))
        {
            comicLink = $"{TitleHumanReadable}/comic.svg";
            return true;
        }

        comicLink = null;
        return false;
    }
}
