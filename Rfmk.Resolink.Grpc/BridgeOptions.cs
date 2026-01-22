using System.ComponentModel.DataAnnotations;

namespace Rfmk.Resolink.Grpc;

public class BridgeOptions
{
    [Required] public Uri HostUrl { get; set; } = null!;
}
