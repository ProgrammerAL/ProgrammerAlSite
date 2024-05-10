using DynamicContentUpdater;

using ProgrammerAl.Site.DynamicContentUpdater;

using System;

using Xunit;

namespace UnitTests.ProgrammerAl.DeveloperSideQuests.Utilities
{
    public class BlogPostParserTests
    {
        private readonly RuntimeConfig _runtimeConfig;

        public BlogPostParserTests()
        {
            _runtimeConfig = new RuntimeConfig
            {
                StorageUrl = "https://MyLink.com"
            };
        }

        [Fact]
        public void WhenParsingValidEntry_AssertTitle()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
            Assert.Equal("Starting This Blog", result.Title);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertPublishedDate()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
            Assert.Equal(new DateOnly(2017, 1, 16), result.ReleaseDate);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertTags()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
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

            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
            Assert.Equal("### The Post!!!" + Environment.NewLine +
                         "Everything else goes here and should be found" + Environment.NewLine
                         + "https://MyLink.com/a/b/c.html" + Environment.NewLine
                         + "https://MyLink.com/1/2/3.html"
                         , result.Post);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertFirstParagraph()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
            Assert.Equal("Everything else goes here and should be found", result.FirstParagraph);
        }

        [Fact]
        public void WhenParsingValidEntryWithSpacingBeforeFirstParagraph_AssertFirstParagraph()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPostWithSpacingInFirstParagraph, "20240501_MyPost");
            Assert.Equal("Everything else goes here and should be found", result.FirstParagraph);
        }

        [Fact]
        public void WhenParsingValidEntryWithManySpacingsBeforeFirstParagraph_AssertFirstParagraph()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPostWithManySpacingsBeforeFirstParagraph, "20240501_MyPost");
            Assert.Equal("Everything else goes here and should be found", result.FirstParagraph);
        }

        [Fact]
        public void WhenParsingValidEntryWithShortPost_AssertFirstParagraph()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPostShortPost, "20240501_MyPost");
            Assert.Equal("Quick test", result.FirstParagraph);
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

        private const string ValidPostWithSpacingInFirstParagraph = @"Title: Starting This Blog
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

        private const string ValidPostWithManySpacingsBeforeFirstParagraph = @"Title: Starting This Blog
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

        private const string ValidPostShortPost = @"Title: Starting This Blog
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

Quick test

";

    }
}
