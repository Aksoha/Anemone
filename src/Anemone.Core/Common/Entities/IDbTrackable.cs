using System;
using System.ComponentModel.DataAnnotations;

namespace Anemone.Core.Common.Entities;

public interface IDbTrackableEntity : IDbEntity
{
    [Required] DateTime? CreationDate { get; set; }
    DateTime? ModificationDate { get; set; }
}