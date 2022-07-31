using System.Collections.Immutable;
using LibProcess2;

namespace ServiceMonitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ImmutableArray<IProcessSearcher> _processSearchers;
    private readonly ProcessRunner _processRunner;

    private record ProcessData(int Id, ProcessInfo Info, Task<int> Task);

    public Worker(ILoggerFactory loggerFactory, IEnumerable<IProcessSearcher> processSearchers)
    {
        _logger = loggerFactory.CreateLogger<Worker>();
        _processSearchers = processSearchers.ToImmutableArray();
        _processRunner = new ProcessRunner(loggerFactory.CreateLogger<ProcessRunner>());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processes = _processSearchers
            .SelectMany(ps => ps.FindProcesses())
            .Select((p,n) => new ProcessData(n, p, StartProcess(p, stoppingToken)))
            .ToArray();
        if (processes.Length == 0)
        {
            _logger.LogWarning("No processes found");
            return;
        }
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = processes.Select(it => it.Task).ToArray();
                var idx = Task.WaitAny(tasks, stoppingToken);
                _logger.LogWarning("Process {FileName} has stopped", processes[idx].Info.Name);
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                processes[idx] = new ProcessData(idx, processes[idx].Info,
                    StartProcess(processes[idx].Info, stoppingToken));
            }
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException) throw;
        }

        foreach (var processData in processes)
        {
            // ReSharper disable once MethodSupportsCancellation
            await processData.Task.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        }

    }

    private Task<int> StartProcess(ProcessInfo p, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting process {FileName}", p.Name);
        return _processRunner.Run(p.FileName, p.Arguments, p.WorkingDirectory,
            s => _logger.LogDebug("{FileName}: {Message}", p.FileName, s),
            s => _logger.LogDebug("{FileName}: E {Message}", p.FileName, s),
            cancellationToken);
    }
}