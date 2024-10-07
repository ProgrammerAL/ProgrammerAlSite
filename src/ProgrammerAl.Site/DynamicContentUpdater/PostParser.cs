#nullable enable
using DynamicContentUpdater.Entities;
using DynamicContentUpdater.Utilities;

using ProgrammerAl.Site.DynamicContentUpdater;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using ExCSS;
using System.Linq;

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
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

            int headerTextLength = rawEntry.IndexOf("---");
            var headerText = rawEntry.Substring(0, headerTextLength);

            try
            {
                var properties = deserializer.Deserialize<PostPropertiesDto>(headerText);
                AssertProperties(properties);

                var tags = properties!.Tags!.Select(x => x!).ToImmutableArray();
                var presentations = properties
                    .Presentations
                    ?.Select(x => new ParsedEntry.PresentationEntry(x!.Id!.Value, x.SlidesUrl, x.SlideImagesUrl))
                    .ToImmutableArray()
                    ?? ImmutableArray<ParsedEntry.PresentationEntry>.Empty;

                var postStartIndex = headerTextLength + 4;
                ReadOnlySpan<char> postSpan = rawEntry.AsSpan(postStartIndex).Trim();
                string post = SanitizePost(postSpan, postName);

                string firstParagraphOfPost = GrabFirstParagraphOfPost(post);

                return new ParsedEntry(
                    properties.Title!,
                    properties.Published!.Value,
                    tags,
                    presentations,
                    post,
                    firstParagraphOfPost);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AssertProperties(PostPropertiesDto? properties)
        {
            if (properties is null)
            {
                throw new Exception($"Post properties object is null");
            }

            AssertProperty(properties.Title, nameof(properties.Title));
            AssertProperty(properties.Published, nameof(properties.Published));
            AssertProperty(properties.Tags, nameof(properties.Tags));

            //Presentations is optional
            if (properties.Presentations is object)
            {
                foreach (var presentation in properties.Presentations)
                {
                    if (presentation is null)
                    {
                        throw new Exception($"Post presentation is null");
                    }

                    AssertProperty(presentation.Id, nameof(presentation.Id));
                    AssertProperty(presentation.SlidesUrl, nameof(presentation.SlidesUrl));
                    AssertProperty(presentation.SlideImagesUrl, nameof(presentation.SlideImagesUrl));
                }
            }
        }

        private void AssertProperty(string? value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception($"Post property is null or empty: {name}");
            }
        }

        private void AssertProperty(int? value, string name)
        {
            if (!value.HasValue
                || value.Value == 0)
            {
                throw new Exception($"Post property is null or empty: {name}");
            }
        }

        private void AssertProperty(DateOnly? value, string name)
        {
            if (!value.HasValue)
            {
                throw new Exception($"Post property is null or empty: {name}");
            }
        }

        private void AssertProperty(string?[]? value, string name)
        {
            if (value is null
                || value.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new Exception($"Post array property is null has a null or empty item: {name}");
            }
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
    }

    public class PostPropertiesDto
    {
        public string? Title { get; set; }
        public DateOnly? Published { get; set; }
        public string?[]? Tags { get; set; }
        public PresentationDto?[]? Presentations { get; set; }

        public class PresentationDto
        {
            public int? Id { get; set; }
            public string? SlidesUrl { get; set; }
            public string? SlideImagesUrl { get; set; }
        }
    }
}
