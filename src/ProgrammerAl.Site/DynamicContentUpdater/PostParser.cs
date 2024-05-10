using DynamicContentUpdater.Entities;
using DynamicContentUpdater.Utilities;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.Pages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DynamicContentUpdater
{
    public class PostParser
    {
        private readonly RuntimeConfig _runtimeConfig;

        public PostParser(RuntimeConfig runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
        }

        public ParsedEntry ParseFromMarkdown(string rawEntry, string postName)
        {
            //Assumes a specific schema
            //  Line 1: Title: <TITLE HERE>
            //  Line 2: Published: <YYYY/MM/DD>
            //  Line 3: Tags:
            //  Line 4-??: - <Tag Name>
            //  Header Ending: --- <Yes, 3 dashes>
            //  ## <Some header. Usually "## Receiving the Quest">
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
            DateOnly publishedDate = ParseDateTimeFromLine(publishedDateLine);

            int tagsLineStartIndex = rawEntry.IndexOf('\n', publishedDateEndIndex);
            int endOfTagsLineStartIndex = rawEntry.IndexOf('\n', tagsLineStartIndex);//Do this again because we want to get to the end of the Tags line
            int tagsLineEndIndex = rawEntry.IndexOf("---", endOfTagsLineStartIndex + 3);
            int tagsLineEndLength = tagsLineEndIndex - tagsLineStartIndex;
            ReadOnlySpan<char> tagLines = rawEntry.AsSpan(tagsLineStartIndex, tagsLineEndLength);
            var tags = ParseTagsFromLines(tagLines);

            int headerCloseIndexStart = rawEntry.IndexOf("---") + 3;
            ReadOnlySpan<char> postSpan = rawEntry.AsSpan(headerCloseIndexStart + 1).Trim();
            string post = SanitizePost(postSpan, postName);

            string firstParagraphOfPost = GrabFirstParagraphOfPost(post);

            return new ParsedEntry(title, publishedDate, tags, post, firstParagraphOfPost);
        }

        private string GrabFirstParagraphOfPost(string post)
        {
            string headerText = "##";
            int firstHeaderStartIndex = post.IndexOf(headerText);
            if (firstHeaderStartIndex == -1)
            {
                return string.Empty;
            }

            //Skip to the end of the header text
            var endOfHeaderIndex = post.IndexOf('\n', firstHeaderStartIndex);
            var startIndex = endOfHeaderIndex + 1;
            //Skip any whitespace characters
            //  A lot of posts have an empty line after the header, so we want to skip that line
            while (char.IsWhiteSpace(post[startIndex]) && startIndex < post.Length)
            {
                startIndex++;
            }

            if (startIndex == post.Length)
            {
                return string.Empty;
            }

            var firstParagraphMarkdown = GrabTextUntilNextEndOfLine(post, startIndex);

            return firstParagraphMarkdown;
        }

        private string GrabTextUntilNextEndOfLine(string post, int startIndex)
        {
            int endIndex = post.IndexOf('\n', startIndex);
            if (endIndex == -1)
            {
                endIndex = post.Length;
            }

            int length = endIndex - startIndex;
            var textSpan = post.AsSpan(startIndex, length).Trim();
            return textSpan.ToString();
        }

        private string SanitizePost(ReadOnlySpan<char> postSpan, string postName)
        {
            var postText = postSpan.ToString();
            postText = HtmlModificationUtility.ReplacePlaceholders(
                postText,
                _runtimeConfig,
                postName);

            return postText;
        }

        private ReadOnlyCollection<string> ParseTagsFromLines(ReadOnlySpan<char> tagLines)
        {
            var tags = new List<string>();
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

            return tags.AsReadOnly();
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

        private DateOnly ParseDateTimeFromLine(ReadOnlySpan<char> textLine)
        {
            string dateTimeString = ParseStringValueFromLine(textLine);
            return DateOnly.ParseExact(dateTimeString, format: "yyyy/MM/dd", provider: null);
        }
    }
}
