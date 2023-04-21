using System.ComponentModel.DataAnnotations;
using Anemone.Core;

namespace Anemone.UI.DataImport.Models;

public enum HeatingSystemColumnMappingModel
{
    Frequency,
    Temperature,

    [Display(Name = "Resistance(frequency)")]
    [ChainedValidation<HeatingSystemColumnMappingModel>(new[] { InductanceF })]
    ResistanceF,

    [Display(Name = "Resistance(temperature)")]
    [ChainedValidation<HeatingSystemColumnMappingModel>(new[] { InductanceT })]
    ResistanceT,

    [Display(Name = "Inductance(frequency)")]
    [ChainedValidation<HeatingSystemColumnMappingModel>(new[] { ResistanceF })]
    InductanceF,

    [Display(Name = "Inductance(temperature)")]
    [ChainedValidation<HeatingSystemColumnMappingModel>(new[] { ResistanceT })]
    InductanceT
}