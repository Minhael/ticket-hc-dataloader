using HotChocolate.DataLoader.DataLoaders;
using HotChocolate.DataLoader.Repositories;

namespace HotChocolate.DataLoader.Resolvers;

[ExtendObjectType(OperationTypeNames.Query)]
public class FooResolver
{
    public Task<IEnumerable<Foo>> GetFooAsync(
        int size,
        [Service] FooRepository repository,
        CancellationToken token = default
    )
    {
        return repository.LoadAsync(Enumerable.Range(0, size), token);
    }
}

[ExtendObjectType(typeof(Foo))]
public class FooExtensions
{
    public async Task<IEnumerable<Bar>> GetBarsAsync([Parent] Foo parent, [DataLoader] BarDataLoader loader, CancellationToken token = default)
    {
        return await loader.LoadAsync(parent.Index, token);
    }
}