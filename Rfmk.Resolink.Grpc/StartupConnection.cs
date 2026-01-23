using Microsoft.Extensions.Options;
using Rfmk.Resolink.Grpc.Link;

namespace Rfmk.Resolink.Grpc;

public class StartupConnection(
    Connection connection,
    IOptions<BridgeOptions> opts,
    IHostLifetime lifetime,
    ILogger<StartupConnection> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var targetURl = opts.Value.HostUrl ?? (opts.Value.Port != null
            ? new Uri($"ws://localhost:{opts.Value.Port}")
            : throw new InvalidOperationException("Either HostUrl or Port must be set."));

        try
        {
            await connection.ConnectAsync(targetURl, cancellationToken);
            logger.LogInformation("Connected to ResoLink socket.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to connect to ResoLink socket.");
            await lifetime.StopAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
