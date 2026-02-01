namespace Rfmk.Resolink.Grpc;

public static class ProtoExtensions
{
    public static Google.Protobuf.WellKnownTypes.Empty ToEmpty(this BatchResponse resp)
        => resp.Queries.Count == 0
            ? new()
            : throw new ArgumentException(
                "An internal batch was expected to contain no queries, but a query was returned.");
}
