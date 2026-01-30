namespace Rfmk.Resolink.Grpc.Projectors;

public interface IBatchProjector
{
    void Project(BatchRequest request);
}
