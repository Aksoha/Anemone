using System.Data;

namespace Anemone.DataImport.Models;

public class Sheet
{
    public required string Name { get; set; }
    public required DataView Set { get; set; }
}