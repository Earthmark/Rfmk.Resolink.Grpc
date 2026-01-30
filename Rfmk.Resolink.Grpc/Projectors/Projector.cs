namespace Rfmk.Resolink.Grpc.Projectors;

public class Projector
{
    private readonly List<IBatchProjector> _projectors =
    [
        new DebugNumberFallback(),
        new CreateExpansion()
    ];

    public static void Project(BatchRequest request)
    {
        foreach (var projector in new Projector()._projectors)
        {
            projector.Project(request);
        }
    }
}
