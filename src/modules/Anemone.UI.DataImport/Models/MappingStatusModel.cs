using System.ComponentModel.DataAnnotations;

namespace Anemone.UI.DataImport.Models;

public enum MappingStatusModel
{
    [Display(Description = "column not mapped")]
    NotAssigned,
    [Display(Description = "")] Ok,
    [Display(Description = "missing row")] MissingRow,

    [Display(Description = "inconsistent mapping data")]
    InconsistentData
}