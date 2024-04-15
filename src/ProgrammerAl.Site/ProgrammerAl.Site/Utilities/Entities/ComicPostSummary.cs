using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.Utilities.Entities;

public class ComicPostSummary
{
    public string Title { get; set; }
    public DateOnly PostedDate { get; set; }
    public string TitleLink { get; set; }
    public int PostNumber { get; set; }
    public string ImageLink { get; set; }
}

