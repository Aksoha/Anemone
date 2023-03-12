using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Anemone.Repository.HeatingSystemData;

public class HeatingSystem : IDbTrackableEntity
{
    [MaxLength(200)] public required string Name { get; set; }
    public required List<HeatingSystemPoint> HeatingSystemPoints { get; set; }
    public int? Id { get; protected internal set; }
    public DateTime? CreationDate { get; protected internal set; }

    public DateTime? ModificationDate { get; protected internal set; }

    public static implicit operator HeatingSystemName(HeatingSystem heatingSystem)
    {
        return new()
            { Id = heatingSystem.Id, Name = heatingSystem.Name };
    }
}