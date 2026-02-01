namespace Rfmk.Resolink.Grpc.Bridge.Converters;

public static class ProtoExtensions
{
    public static Google.Protobuf.WellKnownTypes.Empty ToEmpty(this ResoniteLink.Response resp)
        => resp.GetType() == typeof(ResoniteLink.Response)
            ? new()
            : throw new ArgumentException(
                "A complex response was returned from Resonite, but the bridge assumed it would be simple.");

    public static Google.Protobuf.WellKnownTypes.Empty ToEmpty(this BatchResponse resp)
        => resp.Queries.Count == 0
            ? new()
            : throw new ArgumentException(
                "An internal batch was expected to contain no queries, but a query was returned.");
}
