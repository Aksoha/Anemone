using System.Data;

namespace Anemone.DataImport.Models;

internal class Sheet
{
    public required string Name { get; set; }
    public required DataView Set { get; set; }
}