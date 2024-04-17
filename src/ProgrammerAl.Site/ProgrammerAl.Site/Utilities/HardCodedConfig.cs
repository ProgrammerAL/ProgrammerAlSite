namespace ProgrammerAl.Site.Utilities;

public interface IConfig
{
    string SiteContentUrl { get; }
}

public class HardCodedConfig : IConfig
{
    public string SiteContentUrl => "https://programmeralsitecontent.blob.core.windows.net/sitecontent/";
}
