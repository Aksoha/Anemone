namespace Anemone.Core.Navigation;

public class NavigationPanelItem
{
    public required string Header { get; set; }
    public required PackIconKind Icon { get; set; }
    public required string Uri { get; set; }
}