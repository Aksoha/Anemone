using System.Runtime.InteropServices;

namespace Anemone.Startup;

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
}