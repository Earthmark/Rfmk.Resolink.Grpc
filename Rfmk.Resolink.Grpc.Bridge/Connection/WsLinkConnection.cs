using Grpc.Core;
using ResoniteLink;
using Rfmk.Resolink.Grpc.Bridge.Converters;
using Rfmk.Resolink.Grpc.Projectors;

namespace Rfmk.Resolink.Grpc.Bridge.Connection;

public class WsLinkConnection(WsAdapter c) : ILinkConnection
{
    private static TResponse Convert<TResponse>(Response val) where TResponse : Response =>
        (TResponse)val ?? throw new RpcException(new Status(StatusCode.InvalidArgument,
            "Did not get a data field back from ResoLink."));

    public async Task<BatchResponse> SendBatchAsync(BatchRequest request, CancellationToken cancelToken = default)
    {
        List<Task<Response>> mutations = [];
        foreach (var mut in request.Mutations)
        {
            var mutMessage = mut.ToModel();
            if (mutMessage != null)
            {
                mutations.Add(await c.SendAsync(mutMessage, cancelToken));
            }
        }

        List<Task<Response>> queries = [];
        foreach (var query in request.Queries)
        {
            var queryMessage = query.ToModel();
            if (queryMessage != null)
            {
                queries.Add(await c.SendAsync(queryMessage, cancelToken));
            }
        }

        await Task.WhenAll(mutations);
        var queryResponses = await Task.WhenAll(queries);

        return new BatchResponse
        {
            Queries = { queryResponses.Select(r => r.ToProto()) }
        };
    }

    public async Task<GetSessionResponse> GetSessionAsync(CancellationToken cancellationToken = default)
        => Convert<SessionData>(await await c.SendAsync(new RequestSessionData(), cancellationToken)).ToProto();

    public async Task<AssetResponse> ImportTextureFileAsync(ImportFileRequest request,
        CancellationToken cancelToken = default) =>
        Convert<AssetData>(await await c.SendAsync(request.ToModelTexture(), cancelToken)).ToProto();

    public async Task<AssetResponse>
        ImportAudioClipFileAsync(ImportFileRequest request, CancellationToken cancelToken = default) =>
        Convert<AssetData>(await await c.SendAsync(request.ToModelAudio(), cancelToken)).ToProto();

    public async Task<AssetResponse> ImportTextureAsync(ImportTextureRequest request,
        CancellationToken cancelToken = default) =>
        Convert<AssetData>(await await c.SendAsync(request.Texture.ToModel(), cancelToken)).ToProto();

    public async Task<AssetResponse>
        ImportMeshAsync(ImportMeshRequest request, CancellationToken cancelToken = default) =>
        Convert<AssetData>(request.MeshKindCase switch
        {
            ImportMeshRequest.MeshKindOneofCase.Json =>
                await await c.SendAsync(request.Json.ToModel(), cancelToken),
            ImportMeshRequest.MeshKindOneofCase.Raw => await await c.SendAsync(request.Raw.ToModel(),
                cancelToken),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument,
                "An unknown mesh import kind was provided (or one was not provided)."))
        }).ToProto();

    public async Task<AssetResponse>
        ImportAudioClipAsync(ImportAudioClipRequest request, CancellationToken cancelToken = default) =>
        Convert<AssetData>(await await c.SendAsync(request.RawClip.ToModel(), cancelToken)).ToProto();
}
