using Anemone.Repository.HeatingSystemData;

namespace Anemone.Algorithms.Models;

public class HeatingSystem
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required HeatingSystemPoint[] Points { get; set; }
}