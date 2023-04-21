using Anemone.UI.Core.Icons;

namespace Anemone.Models;

public class SidebarElement
{
    public required string Header { get; set; }
    public required PackIconKind Icon { get; set; }
    public required string Uri { get; set; }
}