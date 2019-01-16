using CommandLine;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    public class Options
    {
        [Option("ContentPath", Required = true, HelpText = "Path to the site content folder")]
        public string ContentPath { get; set; }
    }
}
