namespace Rfmk.Resolink.Grpc.Projectors;

public interface IBatchProjectorLayer
{
    void Project(BatchRequest request);
}
