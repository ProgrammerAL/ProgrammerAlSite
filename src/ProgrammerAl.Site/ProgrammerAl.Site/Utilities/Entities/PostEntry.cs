using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProgrammerAl.Site.Utilities.Entities;

public class PostEntry
{
    public PostEntry(string title, DateOnly releaseDate, ReadOnlyCollection<string> tags, string post, string firstParagraph)
    {
        Title = title;
        ReleaseDate = releaseDate;
        Tags = tags;
        Post = post;
        FirstParagraph = firstParagraph;
    }

    public string Title { get; }
    public DateOnly ReleaseDate { get; }
    public ReadOnlyCollection<string> Tags { get; }
    public string Post { get; }
    public string FirstParagraph { get; }
}
