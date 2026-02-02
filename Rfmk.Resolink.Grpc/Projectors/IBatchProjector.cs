namespace Rfmk.Resolink.Grpc.Projectors;

public interface IBatchProjector
{
    void PrepareRequest(BatchRequest request);
    void PrepareResponse(BatchRequest request, BatchResponse response);
}
