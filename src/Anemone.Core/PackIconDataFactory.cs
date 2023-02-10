using System.Collections.Generic;

namespace Anemone.Core;

// taken from https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MaterialDesignThemes.Wpf/PackIconDataFactory.cs
internal static class PackIconDataFactory
{
    public static IDictionary<PackIconKind, string> Create()
    {
        return new Dictionary<PackIconKind, string>
        {
            { PackIconKind.HamburgerMenu, "M3,6H21V8H3V6M3,11H21V13H3V11M3,16H21V18H3V16Z" }
        };
    }
}