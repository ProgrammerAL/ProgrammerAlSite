using CommandLine;

namespace ProgrammerAl.Site.DynamicContentUpdater
{
    public class RuntimeConfig
    {
        [Option("AppRootPath", Required = true, HelpText = "Path to the root folder holding all files of this application")]
        public string AppRootPath { get; set; }

        [Option("StorageUrl", Required = true, HelpText = "Base URL to the Storage API")]
        public string StorageUrl { get; set; }

        [Option("OutputDirectory", Required = true, HelpText = "Local path to output everything to")]
        public string OutputDirectory { get; set; }
    }
}
