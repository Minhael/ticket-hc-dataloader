using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader.Repositories;

public record Foo
{
    public int Index { get; init; }
}

public class FooRepository
{
    private readonly Tracer _tracer = Measure.CreateTracer<FooRepository>();

    public async Task<IEnumerable<Foo>> LoadAsync(IEnumerable<int> keys, CancellationToken token = default)
    {
        var arr = keys.ToArray();
        using var span = _tracer.StartActiveSpan($"LOAD FOO({arr.Length})");
        return arr.Select(x => new Foo { Index = x });
    }
}