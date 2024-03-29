﻿using System;
using System.Collections.ObjectModel;

namespace ProgrammerAl.Site.Utilities.Entities;

public class BlogPostSummary
{
    public string Title { get; set; }
    public DateOnly PostedDate { get; set; }
    public string TitleLink { get; set; }
    public string FirstParagraph { get; set; }
    public int PostNumber { get; set; }
    public string[] Tags { get; set; }
}
