using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammerAl.Site.Utilities.Entities
{
    public class BlogPostSummary
    {
        public string Title { get; set; }
        public DateTime PostedDate { get; set; }
        public string TitleLink { get; set; }
        public string FirstParagraph { get; set; }
        public int PostNumber { get; set; }
    }
}
