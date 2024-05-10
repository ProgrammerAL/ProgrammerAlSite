using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.DynamicContentUpdater;
using ProgrammerAl.Site.PostDataEntities;

namespace DynamicContentUpdater.Utilities;

public static class HtmlModificationUtility
{
    public static string ReplacePlaceholders(
        string text,
        RuntimeConfig runtimeConfig,
        PostEntry postEntry)
    {
        return ReplacePlaceholders(text, runtimeConfig, postEntry.TitleLink);
    }

    public static string ReplacePlaceholders(
        string text,
        RuntimeConfig runtimeConfig,
        string postTitleLink)
    {
        text = text.Replace("__StorageSiteUrl__", runtimeConfig.StorageUrl);

        var imagePostsUrl = $"{runtimeConfig.StorageUrl}/storage/posts/{postTitleLink}/images";
        text = text.Replace("__PostImagesUrl__", imagePostsUrl);

        return text;
    }
}
