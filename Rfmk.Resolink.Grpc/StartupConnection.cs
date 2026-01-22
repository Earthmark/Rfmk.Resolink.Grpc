using Microsoft.Extensions.Options;
using ResoniteLink;

namespace Rfmk.Resolink.Grpc;

public class StartupConnection(
    LinkInterface link,
    IOptions<BridgeOptions> opts,
    IHostLifetime lifetime,
    ILogger<StartupConnection> logger
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await link.Connect(opts.Value.HostUrl, cancellationToken);
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
