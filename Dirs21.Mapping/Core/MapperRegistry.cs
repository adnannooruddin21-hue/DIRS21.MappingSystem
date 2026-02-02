using System.Reflection;
using Dirs21.Mapping;

public class MapperRegistry
{
    private readonly Dictionary<(Type Source, Type Target), IObjectMapper> _mappers = new();

    /// <summary>
    /// Register a generic mapper
    /// </summary>
    public void Register<TSource, TTarget>(IObjectMapper<TSource, TTarget> mapper)
    {
        var adapter = new ObjectMapperAdapter<TSource, TTarget>(mapper);
        Register(adapter);
    }

    /// <summary>
    /// Register a non-generic mapper (internal use)
    /// </summary>
    public void Register(IObjectMapper mapper)
    {
        var key = (mapper.SourceType, mapper.TargetType);

        if (_mappers.ContainsKey(key))
            throw new InvalidOperationException(
                $"Mapper already registered for {mapper.SourceType.Name} → {mapper.TargetType.Name}");

        _mappers[key] = mapper;
    }

    /// <summary>
    /// Resolve a mapper by type parameters
    /// </summary>
    public IObjectMapper<TSource, TTarget> Resolve<TSource, TTarget>()
    {
        var key = (typeof(TSource), typeof(TTarget));

        if (!_mappers.TryGetValue(key, out var mapper))
            throw new NotSupportedException(
                $"No mapper found for {typeof(TSource).Name} → {typeof(TTarget).Name}");

        return new TypedMapperWrapper<TSource, TTarget>(mapper);
    }

    /// <summary>
    /// Resolve a mapper by types (internal use)
    /// </summary>
    public IObjectMapper Resolve(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);

        if (!_mappers.TryGetValue(key, out var mapper))
            throw new NotSupportedException(
                $"No mapper found for {sourceType.Name} → {targetType.Name}");

        return mapper;
    }

    /// <summary>
    /// Auto-register all mappers from an assembly
    /// </summary>
    public void RegisterFromAssembly(Assembly assembly)
    {
        var mapperInterface = typeof(IObjectMapper<,>);
        
        var mapperTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                         i.GetGenericTypeDefinition() == mapperInterface));

        foreach (var mapperType in mapperTypes)
        {
            var instance = Activator.CreateInstance(mapperType);
            if (instance != null)
            {
                // Find the IObjectMapper<,> interface
                var genericInterface = mapperType.GetInterfaces()
                    .First(i => i.IsGenericType && 
                               i.GetGenericTypeDefinition() == mapperInterface);
                
                var typeArgs = genericInterface.GetGenericArguments();
                var adapterType = typeof(ObjectMapperAdapter<,>).MakeGenericType(typeArgs);
                var adapter = (IObjectMapper)Activator.CreateInstance(adapterType, instance)!;
                
                Register(adapter);
            }
        }
    }

    /// <summary>
    /// Get all registered mapper types
    /// </summary>
    public IEnumerable<(Type Source, Type Target)> GetRegisteredMappers()
    {
        return _mappers.Keys;
    }
}

/// <summary>
/// Adapter to convert generic mapper to non-generic
/// </summary>
internal class ObjectMapperAdapter<TSource, TTarget> : IObjectMapper
{
    private readonly IObjectMapper<TSource, TTarget> _mapper;

    public ObjectMapperAdapter(IObjectMapper<TSource, TTarget> mapper)
    {
        _mapper = mapper;
    }

    public Type SourceType => typeof(TSource);
    public Type TargetType => typeof(TTarget);

    public object Map(object source)
    {
        return _mapper.Map((TSource)source)!;
    }
}

/// <summary>
/// Wrapper to provide generic interface over non-generic mapper
/// </summary>
internal class TypedMapperWrapper<TSource, TTarget> : IObjectMapper<TSource, TTarget>
{
    private readonly IObjectMapper _mapper;

    public TypedMapperWrapper(IObjectMapper mapper)
    {
        _mapper = mapper;
    }

    public TTarget Map(TSource source)
    {
        return (TTarget)_mapper.Map(source!)!;
    }
}
