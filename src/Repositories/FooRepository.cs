namespace HotChocolate.DataLoader.Repositories;

public record Foo
{
    public int Index { get; init; }
}

public class FooRepository
{
    public async Task<IEnumerable<Foo>> LoadAsync(IEnumerable<int> keys, CancellationToken token = default)
    {
        await Task.Delay(1000);
        return keys.Select(x => new Foo { Index = x });
    }
}