namespace Rfmk.Resolink.Grpc.Projectors;

public class BatchProjector : IBatchProjector
{
    private readonly List<IBatchProjectorLayer> _projectors =
    [
        new DebugNumberFallback(),
        new CreateExpansion()
    ];

    public void Project(BatchRequest request)
    {
        foreach (var projector in _projectors)
        {
            projector.Project(request);
        }
    }
}
