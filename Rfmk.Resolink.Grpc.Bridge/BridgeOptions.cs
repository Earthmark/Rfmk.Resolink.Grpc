using Microsoft.Extensions.Options;

namespace Rfmk.Resolink.Grpc.Bridge;

public class BridgeOptions
{
    public Uri? HostUrl { get; set; } = null;

    public int? Port { get; set; } = null;
}

public class BridgeOptionsValidator : IValidateOptions<BridgeOptions>
{
    public ValidateOptionsResult Validate(string? name, BridgeOptions options)
    {
        if (options.HostUrl != null || options.Port != null) { return ValidateOptionsResult.Success; }

        return ValidateOptionsResult.Fail("Either HostUrl or Port must be set.");
    }
}
