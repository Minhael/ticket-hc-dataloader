using HotChocolate.DataLoader.DataLoaders;
using HotChocolate.DataLoader.Repositories;
using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader.src.Resolvers;

public record BatchedFoos
{
    public IEnumerable<Foo> Foos { get; init; } = Enumerable.Empty<Foo>();
}

[ExtendObjectType(OperationTypeNames.Query)]
public class BatchResolver
{
    private readonly Tracer _tracer = Measure.CreateTracer<BatchResolver>();

    public async Task<BatchedFoos> GetFooBatchAsync(
        int size,
        [Service] FooRepository repository,
        CancellationToken token = default
    )
    {
        using var span = _tracer.StartActiveSpan($"RESOLVE FOO({size})");
        return new BatchedFoos { Foos = await repository.LoadAsync(Enumerable.Range(0, size), token) };
    }
}

[ExtendObjectType(typeof(BatchedFoos))]
public class BatchedFoosExtensions
{
    private readonly Tracer _tracer = Measure.CreateTracer<BatchedFoosExtensions>();

    public async Task<IEnumerable<Bar>> GetBarsAsync([Parent] BatchedFoos parent, BarDataLoader loader, CancellationToken token = default)
    {
        using var span = _tracer.StartActiveSpan("EXTEND FOO->BAR");
        var result = await loader.LoadAsync(parent.Foos.Select(x => x.Index).ToArray(), token);
        return result.SelectMany(x => x);
    }
}
