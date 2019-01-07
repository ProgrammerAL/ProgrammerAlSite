using System;
using System.Collections.Immutable;

namespace ProgrammerAl.Site.Utilities.Entities
{
    public class BlogPostEntry
    {
        public BlogPostEntry(string title, DateTime releaseDate, ImmutableList<string> tags, string post, string firstParagraph)
        {
            Title = title;
            ReleaseDate = releaseDate;
            Tags = tags;
            Post = post;
            FirstParagraph = firstParagraph;
        }

        public string Title { get; }
        public DateTime ReleaseDate { get; }
        public ImmutableList<string> Tags { get; }
        public string Post { get; }
        public string FirstParagraph { get; }
    }
}
