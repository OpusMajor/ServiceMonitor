using Serilog;
using Serilog.Events;
using ServiceMonitor;

var logFile = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%/FCCL/Logs/servicemonitor.log");
var errorLogFile = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%/FCCL/Logs/servicemonitor.error.log");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        logFile,
        flushToDiskInterval: TimeSpan.FromSeconds(10),
        rollingInterval: RollingInterval.Day)
    .WriteTo.File(
        errorLogFile,
        flushToDiskInterval: TimeSpan.FromSeconds(2),
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddHostedService<Worker>();
        var searchDirectories = ctx.Configuration.GetSection("ScriptDirectories")
            .Get<GlobDescription[]>() ?? Array.Empty<GlobDescription>();
        var pwsh = ctx.Configuration["PWSH"] ?? "pwsh.exe";
        services.AddSingleton<IProcessSearcher>(new ScriptSearcher(searchDirectories, pwsh));
    })
    .UseSerilog()
    .Build();

await host.RunAsync();