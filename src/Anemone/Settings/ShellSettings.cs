using System.Windows;
using Holize.PersistenceFramework.Serialization;

namespace Anemone.Settings;

[JsonSection("MainWindow")]
public class ShellSettings : Holize.PersistenceFramework.Settings
{
    private int _height;
    private int _left;
    private bool _navigationDrawerExpanded;
    private int _top;
    private int _width;
    private WindowState _windowState;

    public required int Height
    {
        get => _height;
        set => SetField(ref _height, value);
    }

    public required int Width
    {
        get => _width;
        set => SetField(ref _width, value);
    }

    public required int Top
    {
        get => _top;
        set => SetField(ref _top, value);
    }

    public required int Left
    {
        get => _left;
        set => SetField(ref _left, value);
    }

    public required WindowState WindowState
    {
        get => _windowState;
        set => SetField(ref _windowState, value);
    }

    public required bool NavigationDrawerExpanded
    {
        get => _navigationDrawerExpanded;
        set => SetField(ref _navigationDrawerExpanded, value);
    }
}