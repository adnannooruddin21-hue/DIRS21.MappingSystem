namespace Dirs21.Mapping;

/// <summary>
/// Represents the result of a mapping operation with validation
/// </summary>
public class MappingResult<T>
{
    /// <summary>
    /// The mapped object
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Whether the mapped object passed validation
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// List of validation errors (empty if valid)
    /// </summary>
    public List<string> Errors { get; }

    public MappingResult(T value, bool isValid, List<string> errors)
    {
        Value = value;
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }
}
