namespace Dirs21.Mapping;

public class MapHandler
{
    private readonly MapperRegistry _registry;

    public MapHandler(MapperRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Type-safe mapping using generics
    /// </summary>
    public TTarget Map<TSource, TTarget>(TSource source)
    {
        var mapper = _registry.Resolve<TSource, TTarget>();
        return mapper.Map(source);
    }

    /// <summary>
    /// Legacy method for dynamic type mapping (kept for backward compatibility)
    /// </summary>
    public object Map(object source, Type sourceType, Type targetType)
    {
        var mapper = _registry.Resolve(sourceType, targetType);
        return mapper.Map(source);
    }

    /// <summary>
    /// Map a collection of objects
    /// </summary>
    public List<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sources)
    {
        if (sources == null)
            throw new ArgumentNullException(nameof(sources));

        var mapper = _registry.Resolve<TSource, TTarget>();
        return sources.Select(s => mapper.Map(s)).ToList();
    }

    /// <summary>
    /// Map a collection of objects with partner rules applied
    /// </summary>
    public List<TTarget> MapCollection<TSource, TTarget>(
        IEnumerable<TSource> sources,
        IPartnerRules<TTarget> rules)
    {
        if (sources == null)
            throw new ArgumentNullException(nameof(sources));
        if (rules == null)
            throw new ArgumentNullException(nameof(rules));

        var mapper = _registry.Resolve<TSource, TTarget>();
        return sources.Select(s =>
        {
            var target = mapper.Map(s);
            rules.Apply(target);
            return target;
        }).ToList();
    }

    /// <summary>
    /// Map and validate in one operation
    /// </summary>
    public MappingResult<TTarget> MapAndValidate<TSource, TTarget>(
        TSource source,
        IPartnerRules<TTarget> rules)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (rules == null)
            throw new ArgumentNullException(nameof(rules));

        var target = Map<TSource, TTarget>(source);
        rules.Apply(target);
        var isValid = rules.Validate(target, out var errors);

        return new MappingResult<TTarget>(target, isValid, errors);
    }
}
