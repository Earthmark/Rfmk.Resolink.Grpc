namespace Rfmk.Resolink.Grpc.Projectors;

public class BatchProjector(IEnumerable<IBatchProjectorLayer> additionalProjectors) : IBatchProjector
{
    private readonly List<IBatchProjectorLayer> _builtin =
    [
        new DebugNumberFallback(),
        new CreateExpansion()
    ];

    public void PrepareRequest(BatchRequest request)
    {
        foreach (var projector in _builtin)
        {
            projector.PrepareRequest(request);
        }

        foreach (var projector in additionalProjectors)
        {
            projector.PrepareRequest(request);
        }
    }

    public void PrepareResponse(BatchRequest request, BatchResponse response)
    {
        foreach (var projector in _builtin)
        {
            projector.PrepareResponse(request, response);
        }

        foreach (var projector in additionalProjectors)
        {
            projector.PrepareResponse(request, response);
        }
    }
}
