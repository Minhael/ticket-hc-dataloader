using System.Reflection;
using OpenTelemetry.Trace;

namespace HotChocolate.DataLoader;

public static class Measure
{
    public static Tracer CreateTracer<T>(string? version = null)
    {
        var self = typeof(T);
        return TracerProvider.Default.GetTracer(self.FullName, self.GetVersion() ?? version ?? "0.0.0.0");
    }

    private static string? GetVersion(this Type self)
    {
        return self.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
    }
}