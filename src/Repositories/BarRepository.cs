namespace HotChocolate.DataLoader.Repositories;

public record Bar
{
    public int Parent { get; init; }
    public int Index { get; init; }
}

public class BarRepository
{
    public async Task<IEnumerable<Bar>> LoadAsync(IEnumerable<int> keys, CancellationToken token = default)
    {
        await Task.Delay(1000);
        return keys.SelectMany(x => Enumerable.Range(0, x).Select(y => new Bar { Parent = x, Index = y }));
    }
}