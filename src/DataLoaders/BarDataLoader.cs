using HotChocolate.DataLoader.Repositories;

namespace HotChocolate.DataLoader.DataLoaders;

public class BarDataLoader : GroupedDataLoader<int, Bar>
{
    private readonly BarRepository _repository;

    public BarDataLoader(IBatchScheduler batchScheduler, BarRepository repository, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _repository = repository;
    }

    protected override async Task<ILookup<int, Bar>> LoadGroupedBatchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
    {
        var result = await _repository.LoadAsync(keys, cancellationToken);
        return result.ToLookup(x => x.Parent);
    }
}