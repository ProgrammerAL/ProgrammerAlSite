using CommandLine;

namespace ProgrammerAl.Site.ContentUploader
{
    public class RuntimeConfig
    {
        [Option("ContentDirectory", Required = true, HelpText = "Path to the folder that stores the content we will upload")]
        public string ContentDirectory { get; set; }

        [Option("PulumiStackName", Required = true, HelpText = "Full name of the pulumi stack to get Outputs from")]
        public string PulumiStackName { get; set; }

        [Option("PulumiStackPath", Required = true, HelpText = "Path to the Pulumi code the stack runs from")]
        public string PulumiStackPath { get; set; }
    }
}
