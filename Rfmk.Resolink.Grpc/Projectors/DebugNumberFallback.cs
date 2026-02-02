namespace Rfmk.Resolink.Grpc.Projectors;

public class DebugNumberFallback : IBatchProjectorLayer
{
    public void PrepareRequest(BatchRequest request)
    {
        var i = 0;
        foreach (var mutation in request.Mutations)
        {
            if (string.IsNullOrWhiteSpace(mutation.DebugId))
            {
                mutation.DebugId = $"Mutation {i}";
            }

            i++;
        }
    }

    public void PrepareResponse(BatchRequest request, BatchResponse response)
    {
    }
}
