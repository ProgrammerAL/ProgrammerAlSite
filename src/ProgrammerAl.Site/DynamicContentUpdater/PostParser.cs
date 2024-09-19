using DynamicContentUpdater.Entities;
using DynamicContentUpdater.Utilities;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.Pages;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            //  Line ??: PresentationSlides:
            //  Line ??-??: - <Slide Url>
            //  Header Ending: --- <Yes, 3 dashes>
            //  ## <Some header. Usually "## Receiving the Quest">
            //  First Paragraph
            //  Rest of it: The blog post

            int titleStartIndex = rawEntry.IndexOf("Title:");
            int titleEndIndex = rawEntry.IndexOf("\n", titleStartIndex);
            int titleLineLength = titleEndIndex - titleStartIndex;
            ReadOnlySpan<char> titleLine = rawEntry.AsSpan(titleStartIndex, titleLineLength);
            string title = ParseStringValueFromLine(titleLine);

            int publishedDateStartIndex = rawEntry.IndexOf("Published:");
            int publishedDateEndIndex = rawEntry.IndexOf('\n', publishedDateStartIndex + 1);
            int publishedDateEndLength = publishedDateEndIndex - publishedDateStartIndex;
            ReadOnlySpan<char> publishedDateLine = rawEntry.AsSpan(publishedDateStartIndex, publishedDateEndLength);
            DateOnly publishedDate = ParseDateTimeFromLine(publishedDateLine);

            int tagsLineStartIndex = rawEntry.IndexOf("Tags:");
            var tags = ParseListSection(rawEntry, tagsLineStartIndex);

            int slidesLineStartIndex = rawEntry.IndexOf("Slides:");
            ImmutableArray<string> slideUrls;
            if (slidesLineStartIndex == -1)
            {
                slideUrls = ImmutableArray<string>.Empty;
            }
            else
            {
                slideUrls = ParseListSection(rawEntry, slidesLineStartIndex);
            }

            int headerCloseIndexStart = rawEntry.IndexOf("---") + 3;
            ReadOnlySpan<char> postSpan = rawEntry.AsSpan(headerCloseIndexStart + 1).Trim();
            string post = SanitizePost(postSpan, postName);

            string firstParagraphOfPost = GrabFirstParagraphOfPost(post);

            return new ParsedEntry(
                title,
                publishedDate,
                tags,
                slideUrls,
                post,
                firstParagraphOfPost);
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

        private ImmutableArray<string> ParseListSection(string text, int sectionTitleStartIndex)
        {
            var items = new List<string>();

            var endOfLineIndex = text.IndexOf('\n', sectionTitleStartIndex);
            var endOfHeaderIndex = text.IndexOf("---");

            //int startIndex = endOfLineIndex;
            var nextItemIndex = text.IndexOf('-', endOfLineIndex);
            while (nextItemIndex != -1
                && nextItemIndex < endOfHeaderIndex)
            {
                var startIndex = nextItemIndex;
                if (text[startIndex] != '-')
                {
                    break;
                }

                startIndex++;//Skip the dash
                var endIndex = text.IndexOf('\n', startIndex + 1);

                var length = endIndex - startIndex;
                var line = text.Substring(startIndex, length).Trim();

                items.Add(line);

                nextItemIndex = endIndex + 1;
            }

            //while ((startIndex = localSpan.IndexOf('-') + 1) > 0)
            //{
            //    localSpan = localSpan.Slice(startIndex);

            //    int endIndex = localSpan.IndexOf('\n');
            //    string tag = localSpan.Slice(0, endIndex).Trim().ToString();
            //    items.Add(tag);
            //    localSpan = localSpan.Slice(endIndex + 1);
            //}

            return items.ToImmutableArray();
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
