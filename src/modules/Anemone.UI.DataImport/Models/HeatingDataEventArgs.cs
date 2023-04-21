using System;
using System.Collections.Generic;

namespace Anemone.UI.DataImport.Models;

public class HeatingDataEventArgs : EventArgs
{
    public required IEnumerable<HeatingSystemData> FrequencyData { get; set; }
    public required IEnumerable<HeatingSystemData> TemperatureData { get; set; }
}