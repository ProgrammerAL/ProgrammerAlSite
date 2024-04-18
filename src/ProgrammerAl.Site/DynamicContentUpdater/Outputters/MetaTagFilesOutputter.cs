using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgrammerAl.Site.PostDataEntities;
using ProgrammerAl.Site.Utilities.Entities;

namespace DynamicContentUpdater.Outputters;

public class MetaTagFilesOutputter
{
    public void Output(string contentPath, ImmutableArray<PostEntry> allPosts)
    {
        Console.WriteLine($"Outputting meta tag html files...");


        Console.WriteLine($"Completed output of meta tag html files");
    }
}
