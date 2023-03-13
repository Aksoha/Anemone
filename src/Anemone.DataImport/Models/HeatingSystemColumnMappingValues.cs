using System;
using System.Collections.Generic;

namespace Anemone.DataImport.Models;

internal static class HeatingSystemColumnMappingValues
{
    public static readonly IEnumerable<HeatingSystemColumnMappingModel> GetValues =
        Enum.GetValues<HeatingSystemColumnMappingModel>();
}