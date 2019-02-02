using CommandLine;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    public class Options
    {
        [Option("AppRootPath", Required = true, HelpText = "Path to the root folder holding all files of this application")]
        public string AppRootPath { get; set; }
    }
}
