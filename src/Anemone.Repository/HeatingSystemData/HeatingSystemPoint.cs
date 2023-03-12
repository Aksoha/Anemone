using System.ComponentModel.DataAnnotations;

namespace Anemone.Repository.HeatingSystemData;

public class HeatingSystemPoint : IDbEntity
{
    [Required] public HeatingSystemPointType Type { get; set; }
    [Required] public double TypeValue { get; set; }
    [Required] public double Resistance { get; set; }
    [Required] public double Inductance { get; set; }

    [Required] public int? HeatingSystemId { get; protected internal set; }
    public HeatingSystem? HeatingSystem { get; protected internal set; }
    public int? Id { get; protected internal set; }
}