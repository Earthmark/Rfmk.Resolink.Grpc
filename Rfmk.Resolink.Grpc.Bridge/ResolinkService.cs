using Grpc.Core;
using ResoniteLink;
using Rfmk.Resolink.Grpc.Bridge.Connection;
using Rfmk.Resolink.Grpc.Bridge.Converters;
using ProtoEmpty = Google.Protobuf.WellKnownTypes.Empty;

namespace Rfmk.Resolink.Grpc.Bridge;

public class ResolinkService(
    WsAdapter c,
    BatchAdaptor adaptor
) : LinkService.LinkServiceBase
{
    private static TResponse Convert<TResponse>(Response val) where TResponse : Response =>
        (TResponse)val ?? throw new RpcException(new Status(StatusCode.InvalidArgument,
            "Did not get a data field back from ResoLink."));

    public override Task<BatchResponse> ApplyBatch(BatchRequest request, ServerCallContext context)
        => adaptor.SendBatch(request, context.CancellationToken);

    public override async Task<GetSessionResponse> GetSession(ProtoEmpty request,
        ServerCallContext context)
        => Convert<SessionData>(await await c.SendAsync(new RequestSessionData(), context.CancellationToken)).ToProto();

    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context) =>
        Convert<SlotData>(await await c.SendAsync(request.ToModel(), context.CancellationToken)).Data.ToProto();

    public override async Task<ProtoEmpty> AddSlot(AddSlotRequest request,
        ServerCallContext context) =>
        (await adaptor.SendBatch(new BatchRequest
        {
            Mutations = { new BatchMutation { AddSlot = request } }
        }, context.CancellationToken)).ToEmpty();

    public override async Task<ProtoEmpty> UpdateSlot(UpdateSlotRequest request,
        ServerCallContext context) =>
        (await await c.SendAsync(request.ToModel(), context.CancellationToken)).ToEmpty();

    public override async Task<ProtoEmpty> RemoveSlot(DeleteSlotRequest request,
        ServerCallContext context) =>
        (await await c.SendAsync(request.ToModel(), context.CancellationToken)).ToEmpty();

    public override async Task<Component> GetComponent(GetComponentRequest request, ServerCallContext context) =>
        Convert<ComponentData>(await await c.SendAsync(request.ToModel(), context.CancellationToken)).Data.ToProto();

    public override async Task<ProtoEmpty> AddComponent(AddComponentRequest request,
        ServerCallContext context) =>
        (await await c.SendAsync(request.ToModel(), context.CancellationToken)).ToEmpty();

    public override async Task<ProtoEmpty> UpdateComponent(UpdateComponentRequest request,
        ServerCallContext context) =>
        (await await c.SendAsync(request.ToModel(), context.CancellationToken)).ToEmpty();

    public override async Task<ProtoEmpty> RemoveComponent(DeleteComponentRequest request,
        ServerCallContext context) =>
        (await await c.SendAsync(request.ToModel(), context.CancellationToken)).ToEmpty();

    public override async Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context) =>
        Convert<AssetData>(await await c.SendAsync(request.ToModelTexture(), context.CancellationToken)).ToProto();

    public override async Task<AssetResponse>
        ImportAudioClipFile(ImportFileRequest request, ServerCallContext context) =>
        Convert<AssetData>(await await c.SendAsync(request.ToModelAudio(), context.CancellationToken)).ToProto();

    public override async Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context) =>
        Convert<AssetData>(await await c.SendAsync(request.Texture.ToModel(), context.CancellationToken)).ToProto();

    public override async Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context) =>
        Convert<AssetData>(request.MeshKindCase switch
        {
            ImportMeshRequest.MeshKindOneofCase.Json =>
                await await c.SendAsync(request.Json.ToModel(), context.CancellationToken),
            ImportMeshRequest.MeshKindOneofCase.Raw => await await c.SendAsync(request.Raw.ToModel(),
                context.CancellationToken),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument,
                "An unknown mesh import kind was provided (or one was not provided)."))
        }).ToProto();

    public override async Task<AssetResponse>
        ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context) =>
        Convert<AssetData>(await await c.SendAsync(request.RawClip.ToModel(), context.CancellationToken)).ToProto();
}
