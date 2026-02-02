using Rfmk.Resolink.Grpc.Projectors;

namespace Rfmk.Resolink.Grpc.Bridge.Projectors;

public class ExcludeComponentsEnforcer : IBatchProjectorLayer
{
    public void PrepareRequest(BatchRequest request)
    {
    }

    public void PrepareResponse(BatchRequest request, BatchResponse response)
    {
        foreach (var (req, resp) in request.Queries.Zip(response.Queries))
        {
            if (req.QueryCase == BatchQuery.QueryOneofCase.GetSlot && !req.GetSlot.IncludeComponents)
            {
                PruneComponents(resp.Slot);
            }
        }
    }

    private static void PruneComponents(Slot slot)
    {
        slot.Components.Clear();
        foreach (var child in slot.Children)
        {
            PruneComponents(child);
        }
    }
}