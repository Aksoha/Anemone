using System;
using System.Collections.Generic;

namespace Anemone.UI.DataImport.Models;

internal static class HeatingSystemColumnMappingValues
{
    public static readonly IEnumerable<HeatingSystemColumnMappingModel> GetValues =
        Enum.GetValues<HeatingSystemColumnMappingModel>();
}