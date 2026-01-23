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
        try
        {
            await connection.ConnectAsync(opts.Value.HostUrl, cancellationToken);
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
