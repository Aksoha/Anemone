using System;
using System.ComponentModel.DataAnnotations;

namespace Anemone.Repository;

public interface IDbTrackableEntity : IDbEntity
{
    [Required] DateTime? CreationDate { get; }
    DateTime? ModificationDate { get; }
}