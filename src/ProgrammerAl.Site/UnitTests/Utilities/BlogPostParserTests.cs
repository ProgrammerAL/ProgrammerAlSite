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
                StorageUrl = "https://MyLink.com",
                AppRootPath = "C:\\MyPath",
                OutputDirectory = "./"
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
            Assert.Equal(7, result.Tags.Length);
            Assert.Contains("Wyam", result.Tags);
            Assert.Contains("Azure App Service", result.Tags);
            Assert.Contains("VSTS", result.Tags);
            Assert.Contains("Cake", result.Tags);
            Assert.Contains("NuGet", result.Tags);
            Assert.Contains("Continuous Integration", result.Tags);
            Assert.Contains("Continuous Deployment", result.Tags);
        }

        [Fact]
        public void WhenParsingValidEntry_AssertSlides()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost, "20240501_MyPost");
            Assert.Equal(2, result.Presentations.Length);

            Assert.Equal(1, result.Presentations[0].Id);
            Assert.Equal("https://MyLink.com/a/b/c.html", result.Presentations[0].SlidesUrl);
            Assert.Equal("https://MyLink.com/a/b/images", result.Presentations[0].SlideImagesUrl);

            Assert.Equal(2, result.Presentations[1].Id);
            Assert.Equal("https://MyLink.com/1/2/3.html", result.Presentations[1].SlidesUrl);
            Assert.Equal("https://MyLink.com/1/2/images", result.Presentations[1].SlideImagesUrl);
        }

        [Fact]
        public void WhenParsingValidEntryWithNoSlides_AssertNoSlides()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPostNoSlides, "20240501_MyPost");
            Assert.Empty(result.Presentations);
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

        [Fact]
        public void WhenParseingPostWithColonInTitle_AssertTitle()
        {
            var parser = new PostParser(_runtimeConfig);
            var result = parser.ParseFromMarkdown(ValidPost_ColonInTitle, "20240501_MyPost");
            Assert.Equal("My Title: Starting This Blog", result.Title);
        }

        private const string ValidPost = @"
Title: Starting This Blog
Published: 2017/01/16
Tags: 

- Wyam
- Azure App Service
- VSTS
- Cake
- NuGet
- Continuous Integration
- Continuous Deployment

Presentations:
- Id: 1
  SlidesUrl: https://MyLink.com/a/b/c.html
  SlidesImagesUrl: https://MyLink.com/a/b/images
- Id: 2
  SlidesUrl: https://MyLink.com/1/2/3.html
  SlidesImagesUrl: https://MyLink.com/1/2/images
---
### The Post!!!
Everything else goes here and should be found
__StorageSiteUrl__/a/b/c.html
__StorageSiteUrl__/1/2/3.html
";

        private const string ValidPostNoSlides = @"Title: Starting This Blog
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

        private const string ValidPost_ColonInTitle = @"
Title: ""My Title: Starting This Blog""
Published: 2017/01/16
Tags: 
- Wyam
- Continuous Deployment
---
### The Post!!!

Quick test

";

    }
}
