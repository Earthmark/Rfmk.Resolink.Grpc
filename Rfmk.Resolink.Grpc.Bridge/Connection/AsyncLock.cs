namespace Rfmk.Resolink.Grpc.Bridge.Connection;

public class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _sem = new(1, 1);

    public async ValueTask<AsyncLockReleaser> LockAsync(CancellationToken cancellationToken)
    {
        await _sem.WaitAsync(cancellationToken);
        return new AsyncLockReleaser(this);
    }

    public struct AsyncLockReleaser(AsyncLock @lock) : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                @lock?._sem.Release();
            }
        }
    }

    public void Dispose()
    {
        _sem.Dispose();
    }
}
