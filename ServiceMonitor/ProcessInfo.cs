using System.Collections.Immutable;

namespace ServiceMonitor;

public record ProcessInfo(string Name, string FileName, ImmutableList<string> Arguments, string? WorkingDirectory);