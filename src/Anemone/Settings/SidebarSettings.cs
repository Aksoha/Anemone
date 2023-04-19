using Holize.PersistenceFramework.Serialization;

namespace Anemone.Settings;

[JsonSection("Sidebar")]
public class SidebarSettings : Holize.PersistenceFramework.Settings
{
    private bool _isExpanded;
    private double _maxWidth;
    private double _minWidth;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetField(ref _isExpanded, value);
    }

    public double MinWidth
    {
        get => _minWidth;
        set => SetField(ref _minWidth, value);
    }

    public double MaxWidth
    {
        get => _maxWidth;
        set => SetField(ref _maxWidth, value);
    }
}