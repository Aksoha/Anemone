using System;
using System.Data;

namespace Anemone.Algorithms.Models;

/// <summary>
///     defines the overrides for conversion of property to <see cref="DataColumn" />
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExportColumnAttribute : Attribute
{
    public ExportColumnAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Column name.
    /// </summary>
    public string Name { get; }
}