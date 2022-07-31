namespace ServiceMonitor;

public interface IProcessSearcher
{
    IEnumerable<ProcessInfo> FindProcesses();
}