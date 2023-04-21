using System.ComponentModel.DataAnnotations;

namespace Anemone.Core.Common.Entities;

public class HeatingSystemPoint : IDbEntity
{
    [Required] public HeatingSystemPointType Type { get; set; }
    [Required] public double TypeValue { get; set; }
    [Required] public double Resistance { get; set; }
    [Required] public double Inductance { get; set; }

    [Required] public int? HeatingSystemId { get; set; }
    public HeatingSystem? HeatingSystem { get; set; }
    public int? Id { get; set; }
}