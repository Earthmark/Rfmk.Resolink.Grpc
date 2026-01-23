using System.Threading.Channels;
using ResoniteLink;

namespace Rfmk.Resolink.Grpc.Link;

public class Connection : IAsyncDisposable
{
    private readonly ILogger<Connection> _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly LinkInterface _link;
    private readonly Channel<Func<LinkInterface, Task>> _pendingOperations;
    private readonly Task _processPendingOperationsTask;
    
    private Uri? _target;

    public Connection(ILogger<Connection> logger)
    {
        _logger = logger;
        _link = new LinkInterface();
        _pendingOperations = Channel.CreateUnbounded<Func<LinkInterface, Task>>();
        _processPendingOperationsTask = Task.Run(ProcessPendingOperationsAsync);
    }

    public async Task ConnectAsync(Uri target, CancellationToken cancellationToken = default)
    {
        _target = target;
        await using (cancellationToken.Register(() => _cts.Cancel()))
        {
            await _link.Connect(_target, _cts.Token);
        }
    }

    private async Task ProcessPendingOperationsAsync()
    {
        try
        {
            await foreach (var op in _pendingOperations.Reader.ReadAllAsync(_cts.Token))
            {
                await op(_link);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Channel to {} raised an error.", _target);
            _link.Dispose();
        }
    }

    public async Task<T> QueueMessageAsync<T>(Func<LinkInterface, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<T>();
        await _pendingOperations.Writer.WriteAsync(async l =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetCanceled(cancellationToken);
            }
            else
            {
                tcs.SetResult(await operation(l));
            }
        }, cancellationToken);
        return await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        
        _pendingOperations.Writer.Complete();
        await CastAndDispose(_cts);
        await CastAndDispose(_link);

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}