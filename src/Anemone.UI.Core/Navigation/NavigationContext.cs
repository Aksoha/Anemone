namespace Anemone.UI.Core.Navigation;

public class NavigationContext
{
    public NavigationContext(string region, string uri)
    {
        Region = region;
        Uri = uri;
    }

    public string Region { get; }
    public string Uri { get; }
    public bool IsHandled { get; set; }
}