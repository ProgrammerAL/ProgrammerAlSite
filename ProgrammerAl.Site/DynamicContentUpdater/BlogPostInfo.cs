using ProgrammerAl.Site.Utilities.Entities;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    partial class Program
    {
        public class BlogPostInfo
        {
            public BlogPostInfo(string fileNameWithoutExtension, BlogPostEntry entry)
            {
                FileNameWithoutExtension = fileNameWithoutExtension;
                Entry = entry;
            }

            public string FileNameWithoutExtension { get; }
            public BlogPostEntry Entry { get; }
        }
    }
}
