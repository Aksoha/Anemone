using System.Data;

namespace Anemone.UI.DataImport.Models;

public class Sheet
{
    public required string Name { get; set; }
    public required DataView Set { get; set; }
}