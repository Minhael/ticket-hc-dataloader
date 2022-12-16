using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader.Repositories;

public record Bar
{
    public int Parent { get; init; }
    public int Index { get; init; }
}

public class BarRepository
{
    private readonly Tracer _tracer = Measure.CreateTracer<BarRepository>();

    public async Task<IEnumerable<Bar>> LoadAsync(IEnumerable<int> keys, CancellationToken token = default)
    {
        var arr = keys.ToArray();
        using var span = _tracer.StartActiveSpan($"LOAD BAR({arr.Length})");
        return arr.SelectMany(x => Enumerable.Range(0, x).Select(y => new Bar { Parent = x, Index = y }));
    }
}