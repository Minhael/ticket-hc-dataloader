using HotChocolate.DataLoader.Repositories;
using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader.DataLoaders;

public class BarDataLoader : GroupedDataLoader<int, Bar>
{
    private readonly Tracer _tracer = Measure.CreateTracer<BarDataLoader>();

    private readonly BarRepository _repository;

    public BarDataLoader(IBatchScheduler batchScheduler, BarRepository repository, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _repository = repository;
    }

    protected override async Task<ILookup<int, Bar>> LoadGroupedBatchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
    {
        using var span = _tracer.StartActiveSpan($"GROUP BAR({keys.Count})");
        var result = await _repository.LoadAsync(keys, cancellationToken);
        return result.ToLookup(x => x.Parent);
    }
}