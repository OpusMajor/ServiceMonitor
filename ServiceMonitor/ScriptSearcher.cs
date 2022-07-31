using System.Collections.Immutable;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using OpusMajor.FileSystem;

namespace ServiceMonitor;

public class GlobDescription
{
    public string? BaseDirectory { get; set; }
    public string?[]? Globs { get; set; }
}

public class ScriptSearcher : IProcessSearcher
{
    private readonly string _pwsh;
    private readonly ImmutableList<GlobDescription> _globDescriptions;

    public ScriptSearcher(IEnumerable<GlobDescription> globDescriptions, string pwsh)
    {
        _globDescriptions = globDescriptions.ToImmutableList();
        _pwsh = pwsh;
    }

    public IEnumerable<ProcessInfo> FindProcesses()
    {
        var processes = ImmutableList<FileInfo>.Empty;
        foreach (var glob in _globDescriptions)
        {
            if (glob.Globs == null || glob.BaseDirectory == null)
            {
                throw new Exception("Invalid glob description");
            }
            var matcher = new Matcher(StringComparison.InvariantCultureIgnoreCase);
            matcher.AddIncludePatterns(glob.Globs);
            var baseDirectory = new DirectoryInfo(glob.BaseDirectory);
            var result = matcher.Execute(new DirectoryInfoWrapper(baseDirectory));
            processes = processes.AddRange(result.Files.Select(it => baseDirectory.GetFile(it.Path)));
        }

        return processes.Select(it => new ProcessInfo(it.Name, _pwsh, ImmutableList<string>.Empty
            .Add("/c")
            .Add(it.FullName), it.DirectoryName));

    }
}