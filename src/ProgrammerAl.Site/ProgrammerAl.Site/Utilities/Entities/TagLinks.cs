using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.Utilities.Entities;

public class TagLinks
{
    public const string TagLinksFile = "TagLinks.json";
    public Dictionary<string, string[]>? Links { get; set; }
}

