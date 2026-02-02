namespace Dirs21.Mapping;

/// <summary>
/// Generic interface for mapping between source and target types
/// </summary>
public interface IObjectMapper<TSource, TTarget>
{
    TTarget Map(TSource source);
}

/// <summary>
/// Non-generic base interface for internal registry usage
/// </summary>
public interface IObjectMapper
{
    Type SourceType { get; }
    Type TargetType { get; }
    object Map(object source);
}
