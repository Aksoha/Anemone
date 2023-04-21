using System.ComponentModel.DataAnnotations;

namespace Anemone.Core.Common.Entities;

/// <summary>
///     entity stored in the database.
/// </summary>
public interface IDbEntity
{
    /// <summary>
    ///     Id of an object stored in the database.
    /// </summary>
    [Key]
    int? Id { get; set; }
}