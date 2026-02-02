namespace Rfmk.Resolink.Grpc.Projectors;

public interface IBatchProjectorLayer
{
    void PrepareRequest(BatchRequest request);
    void PrepareResponse(BatchRequest request, BatchResponse response);
}
