using System;

namespace Anemone.Core;

/// <summary>
///     Mark an enum for chained validation.
/// </summary>
/// <typeparam name="T">The type of enum.</typeparam>
/// <example>
///     Validator for value "A" should also check if data "B" and "D" is in valid state.
///     <code>
/// public enum ExampleEnum
/// {
///     [ChainedValidationAttribute&lt;ExampleEnum&gt;(new [] { B, D })]
///     A,
///     B,
///     C,
///     D
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field)]
public class ChainedValidationAttribute<T> : Attribute where T : Enum
{
    public ChainedValidationAttribute(T[] validateTogetherWith)
    {
        ValidateTogetherWith = validateTogetherWith;
    }

    /// <summary>
    ///     The values that should also be validated when performing validation on target field.
    /// </summary>
    public T[] ValidateTogetherWith { get; }
}