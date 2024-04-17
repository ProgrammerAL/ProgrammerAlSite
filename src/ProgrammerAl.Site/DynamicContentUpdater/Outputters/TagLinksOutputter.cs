using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DynamicContentUpdater.Entities;

using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class TagLinksOutputter
{
    public void Output(string contentPath, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting {TagLinks.TagLinksFile}...");

        var tagLinks = GenerateTagLinks(allPosts);
        OutputUtils.WriteOutFileAsJson(tagLinks, contentPath, TagLinks.TagLinksFile);

        Console.WriteLine($"Completed output of {TagLinks.TagLinksFile}");
    }

    private TagLinks GenerateTagLinks(ImmutableArray<PostEntry> allPosts)
    {
        var tagLinks = new TagLinks();
        var tagDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var post in allPosts)
        {
            foreach (var tag in post.Tags)
            {
                if (!tagDictionary.ContainsKey(tag))
                {
                    tagDictionary.Add(tag, new List<string>());
                }
                tagDictionary[tag].Add(post.TitleLink);
            }
        }

        tagLinks.Links = tagDictionary.ToDictionary(x => x.Key, x => x.Value.ToArray());
        return tagLinks;
    }
}
