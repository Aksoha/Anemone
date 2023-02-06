using System.Diagnostics.CodeAnalysis;
using Holize.PersistenceFramework.Serialization;

namespace Anemone.Settings;

[JsonSection("Debug:ConsoleWindow")]
[ExcludeFromCodeCoverage(Justification = "used for debugging purposes only")]
public class DebuggingConsoleSettings : Holize.PersistenceFramework.Settings
{
    private int _cx;
    private int _cy;
    private int _x;
    private int _y;

    public required int X
    {
        get => _x;
        set => SetField(ref _x, value);
    }

    public required int Y
    {
        get => _y;
        set => SetField(ref _y, value);
    }

    public required int Cx
    {
        get => _cx;
        set => SetField(ref _cx, value);
    }

    public required int Cy
    {
        get => _cy;
        set => SetField(ref _cy, value);
    }
}