namespace Dirs21.Mapping;

/// <summary>
/// Interface for applying partner-specific business rules and transformations
/// </summary>
public interface IPartnerRules<T>
{
    /// <summary>
    /// Apply partner-specific rules to the mapped object
    /// </summary>
    void Apply(T target);
    
    /// <summary>
    /// Validate the object according to partner-specific rules
    /// </summary>
    bool Validate(T target, out List<string> errors);
}
