using Grpc.Core;
using ResoniteLink;
using Rfmk.Resonite;
using Slot = Rfmk.Resonite.Slot;

namespace Rfmk.Resolink.Grpc.Services;

public class ResolinkService(
    LinkInterface link,
    ILogger<ResolinkService> logger
    ) : Resonite.ResoniteLink.ResoniteLinkBase
{
    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context)
    {
        var response = await link.GetSlotData(new GetSlot
        {
            SlotID = request.SlotId,
            Depth = request.Depth,
            IncludeComponentData = request.IncludeComponentData
        });

        return response.Data?.ToProto() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Did not get a data field back from ResoLink."));
    }
}
