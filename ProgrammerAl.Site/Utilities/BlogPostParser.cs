using ProgrammerAl.Site.Utilities.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ProgrammerAl.Site.Utilities
{
    public class BlogPostParser
    {
        private readonly IConfig _config;

        public BlogPostParser(IConfig config)
        {
            _config = config;
        }

        public BlogPostEntry ParseFromMarkdown(string rawEntry)
        {
            //Assumes a specific schema
            //  Line 1: Title: <TITLE HERE>
            //  Line 2: Published: <MM/DD/YYYY>
            //  Line 3: Tags:
            //  Line 4-??: - <Tag Name>
            //  Header Ending: --- <Yes, 3 dashes>
            //  ### <Some header. Usually "### Receiving the Quest">
            //  First Paragraph
            //  Rest of it: The blog post

            int titleStartIndex = 0;
            int titleEndIndex = rawEntry.IndexOf('\n');
            int titleLineLength = titleEndIndex - titleStartIndex;
            ReadOnlySpan<char> titleLine = rawEntry.AsSpan(titleStartIndex, titleLineLength);
            string title = ParseStringValueFromLine(titleLine);


            int publishedDateStartIndex = rawEntry.IndexOf('\n', titleEndIndex);
            int publishedDateEndIndex = rawEntry.IndexOf('\n', publishedDateStartIndex + 1);
            int publishedDateEndLength = publishedDateEndIndex - publishedDateStartIndex;
            ReadOnlySpan<char> publishedDateLine = rawEntry.AsSpan(publishedDateStartIndex, publishedDateEndLength);
            DateTime publishedDate = ParseDateTimeFromLine(publishedDateLine);

            int tagsLineStartIndex = rawEntry.IndexOf('\n', publishedDateEndIndex);
            int endOfTagsLineStartIndex = rawEntry.IndexOf('\n', tagsLineStartIndex);//Do this again because we want to get to the end of the Tags line
            int tagsLineEndIndex = rawEntry.IndexOf("---", endOfTagsLineStartIndex + 3);
            int tagsLineEndLength = tagsLineEndIndex - tagsLineStartIndex;
            ReadOnlySpan<char> tagLines = rawEntry.AsSpan(tagsLineStartIndex, tagsLineEndLength);
            ImmutableList<string> tags = ParseTagsFromLines(tagLines);

            var headerCloseIndexStart = rawEntry.IndexOf("---") + 3;
            var postSpan = rawEntry.AsSpan(headerCloseIndexStart + 1).Trim();
            var post = SanitizePost(postSpan);

            var firstParagraphOfPost = GrabFirstParagraphOfPost(post);

            return new BlogPostEntry(title, publishedDate, tags, post, firstParagraphOfPost);
        }

        private string GrabFirstParagraphOfPost(string post)
        {
            var headerText = "###";
            var headerStartIndex = post.IndexOf(headerText);
            if (headerStartIndex == -1)
            {
                return string.Empty;
            }

            //headerStartIndex += headerText.Length;
            var startIndex = post.IndexOf('\n', headerStartIndex) + 1;
            var endIndex = post.IndexOf('\n', startIndex);
            var length = endIndex - startIndex;
            var firstParagraphSpan = post.AsSpan(startIndex, length).Trim();
            return firstParagraphSpan.ToString();
        }

        private string SanitizePost(ReadOnlySpan<char> postSpan)
        {
            var builder = new StringBuilder(postSpan.ToString());
            builder.Replace("__StorageSiteUrl__", _config.SiteContentUrl);

            return builder.ToString();
        }

        private ImmutableList<string> ParseTagsFromLines(ReadOnlySpan<char> tagLines)
        {
            List<string> tags = new List<string>();
            ReadOnlySpan<char> localSpan = tagLines.Slice(0);

            int startIndex;

            while ((startIndex = localSpan.IndexOf('-') + 1) > 0)
            {
                localSpan = localSpan.Slice(startIndex);

                int endIndex = localSpan.IndexOf('\n');
                string tag = localSpan.Slice(0, endIndex).Trim().ToString();
                tags.Add(tag);
                localSpan = localSpan.Slice(endIndex + 1);
            }

            return tags.ToImmutableList();
        }

        private string ParseStringValueFromLine(ReadOnlySpan<char> textLine)
        {
            int separatorIndex = textLine.IndexOf(':');
            if (separatorIndex > -1)
            {
                separatorIndex++;//index is first character after the colon
                return textLine.Slice(separatorIndex).Trim().ToString();
            }

            return "Unknown Error";
        }

        private DateTime ParseDateTimeFromLine(ReadOnlySpan<char> textLine)
        {
            string dateTimeString = ParseStringValueFromLine(textLine);
            return DateTime.Parse(dateTimeString);
        }
    }
}
