using DynamicContentUpdater;

using NSubstitute;
using ProgrammerAl.Site.Utilities;
using ProgrammerAl.Site.Utilities.Entities;
using System;
using Xunit;

namespace UnitTests.ProgrammerAl.DeveloperSideQuests.Utilities
{
    public class BlogPostParserTests
    {
        private readonly IConfig _config;

        public BlogPostParserTests()
        {
            _config = Substitute.For<IConfig>();
        }

        [Fact]
        public void WhenParsingValidEntry_AssertTitle()
        {
            PostParser parser = new PostParser(_config);
            PostEntry result = parser.ParseFromMarkdown(ValidPost);
            Assert.Equal("Starting This Blog", result.Title);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertPublishedDate()
        {
            PostParser parser = new PostParser(_config);
            PostEntry result = parser.ParseFromMarkdown(ValidPost);
            Assert.Equal(new DateOnly(2017, 1, 16), result.ReleaseDate);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertTags()
        {
            PostParser parser = new PostParser(_config);
            PostEntry result = parser.ParseFromMarkdown(ValidPost);
            Assert.Equal(7, result.Tags.Count);
            Assert.Contains("Wyam", result.Tags);
            Assert.Contains("Azure App Service", result.Tags);
            Assert.Contains("VSTS", result.Tags);
            Assert.Contains("Cake", result.Tags);
            Assert.Contains("NuGet", result.Tags);
            Assert.Contains("Continuous Integration", result.Tags);
            Assert.Contains("Continuous Deployment", result.Tags);
        }
        
        [Fact]
        public void WhenParsingValidEntry_AssertContent()
        {
            _config.SiteContentUrl.Returns("https://MyLink.com");

            PostParser parser = new PostParser(_config);
            PostEntry result = parser.ParseFromMarkdown(ValidPost);
            Assert.Equal("### The Post!!!" + Environment.NewLine +
                         "Everything else goes here and should be found" + Environment.NewLine
                         + "https://MyLink.com/a/b/c.html" + Environment.NewLine
                         + "https://MyLink.com/1/2/3.html"
                         , result.Post);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertFirstParagraph()
        {
            _config.SiteContentUrl.Returns("https://MyLink.com");

            PostParser parser = new PostParser(_config);
            PostEntry result = parser.ParseFromMarkdown(ValidPost);
            Assert.Equal("Everything else goes here and should be found", result.FirstParagraph);
        }

        private const string ValidPost = @"Title: Starting This Blog
Published: 2017/01/16
Tags: 
- Wyam
- Azure App Service
- VSTS
- Cake
- NuGet
- Continuous Integration
- Continuous Deployment
---
### The Post!!!
Everything else goes here and should be found
__StorageSiteUrl__/a/b/c.html
__StorageSiteUrl__/1/2/3.html
";
    }
}
