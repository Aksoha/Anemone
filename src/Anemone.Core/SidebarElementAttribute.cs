using System;

namespace Anemone.Core;

[AttributeUsage(AttributeTargets.Class)]
public class SidebarElementAttribute : Attribute
{
    public SidebarElementAttribute(string header, PackIconKind icon, string uri)
    {
        Header = header;
        Icon = icon;
        Uri = uri;
    }

    public string Header { get; }
    public PackIconKind Icon { get; }
    public string Uri { get; }
}