using System.ComponentModel.DataAnnotations;

namespace Anemone.DataImport.Models;

internal enum HeatingSystemColumnMappingModel
{
    Frequency,
    Temperature,

    [Display(Name = "Resistance(frequency)")]
    ResistanceF,

    [Display(Name = "Resistance(temperature)")]
    ResistanceT,

    [Display(Name = "Inductance(frequency)")]
    InductanceF,

    [Display(Name = "Inductance(temperature)")]
    InductanceT,
}