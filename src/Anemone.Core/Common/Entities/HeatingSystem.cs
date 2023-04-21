using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Anemone.Core.Common.Entities;

public class HeatingSystem : IDbTrackableEntity
{
    [MaxLength(200)] public required string Name { get; set; }
    public required List<HeatingSystemPoint> HeatingSystemPoints { get; set; }
    public int? Id { get; set; }
    public DateTime? CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }

    public static implicit operator HeatingSystemName(HeatingSystem heatingSystem)
    {
        return new HeatingSystemName { Id = heatingSystem.Id, Name = heatingSystem.Name };
    }
}