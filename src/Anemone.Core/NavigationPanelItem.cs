namespace Anemone.Core;

public class NavigationPanelItem
{
    public required string Header { get; set; }
    public required PackIconKind Icon { get; set; }
    public required string NavigationPath { get; set; }
}