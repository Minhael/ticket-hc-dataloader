using HotChocolate.DataLoader.DataLoaders;
using HotChocolate.DataLoader.Repositories;
using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader.Resolvers;

[ExtendObjectType(OperationTypeNames.Query)]
public class FooResolver
{
    private readonly Tracer _tracer = Measure.CreateTracer<FooResolver>();

    public async Task<IEnumerable<Foo>> GetFoosAsync(
        int size,
        [Service] FooRepository repository,
        CancellationToken token = default
    )
    {
        using var span = _tracer.StartActiveSpan($"RESOLVE FOO({size})");
        return await repository.LoadAsync(Enumerable.Range(0, size), token);
    }
}

[ExtendObjectType(typeof(Foo))]
public class FooExtensions
{
    private readonly Tracer _tracer = Measure.CreateTracer<FooExtensions>();

    public async Task<IEnumerable<Bar>> GetBarsAsync([Parent] Foo parent, BarDataLoader loader, CancellationToken token = default)
    {
        using var span = _tracer.StartActiveSpan($"EXTEND FOO->BAR[{parent.Index}]");
        return await loader.LoadAsync(parent.Index, token);
    }
}