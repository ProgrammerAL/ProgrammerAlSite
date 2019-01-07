namespace ProgrammerAl.Site.Utilities
{
    public interface IConfig
    {
        string SiteContentUrl { get; }
    }

    public class Config : IConfig
    {
        public string SiteContentUrl => "https://developersidequestssite.blob.core.windows.net/sitecontent/";
    }
}
