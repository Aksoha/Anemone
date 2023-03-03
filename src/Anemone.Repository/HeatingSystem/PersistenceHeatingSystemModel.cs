using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Anemone.Repository.HeatingSystem;

[JsonConverter(typeof(PersistenceHeatingSystemConverter))]
public class PersistenceHeatingSystemModel : IDbEntity
{
    public required string Name { get; set; }
    public required IEnumerable<HeatingSystemDataPointModel> FrequencyData { get; set; }
    public required IEnumerable<HeatingSystemDataPointModel> TemperatureData { get; set; }
    public string? Id { get; set; }
}