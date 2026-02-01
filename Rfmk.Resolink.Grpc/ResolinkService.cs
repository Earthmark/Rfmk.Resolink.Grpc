using Grpc.Core;
using Rfmk.Resolink.Grpc.Projectors;
using ProtoEmpty = Google.Protobuf.WellKnownTypes.Empty;

namespace Rfmk.Resolink.Grpc;

public class ResolinkService(
    IBatchProjector projector,
    ILinkConnection link
) : LinkService.LinkServiceBase
{
    public override async Task<BatchResponse> ApplyBatch(BatchRequest request, ServerCallContext context)
    {
        projector.Project(request);
        return await link.SendBatchAsync(request, context.CancellationToken);
    }

    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Queries =
            {
                new BatchQuery
                {
                    GetSlot = request
                }
            }
        }, context)).Queries[0].Slot;

    public override async Task<ProtoEmpty> AddSlot(AddSlotRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    AddSlot = request
                }
            }
        }, context)).ToEmpty();

    public override async Task<ProtoEmpty> UpdateSlot(UpdateSlotRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    UpdateSlot = request
                }
            }
        }, context)).ToEmpty();

    public override async Task<ProtoEmpty> RemoveSlot(DeleteSlotRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    DeleteSlot = request
                }
            }
        }, context)).ToEmpty();

    public override async Task<Component> GetComponent(GetComponentRequest request, ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Queries =
            {
                new BatchQuery
                {
                    GetComponent = request
                }
            }
        }, context)).Queries[0].Component;

    public override async Task<ProtoEmpty> AddComponent(AddComponentRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    AddComponent = request
                }
            }
        }, context)).ToEmpty();

    public override async Task<ProtoEmpty> UpdateComponent(UpdateComponentRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    UpdateComponent = request
                }
            }
        }, context)).ToEmpty();

    public override async Task<ProtoEmpty> RemoveComponent(DeleteComponentRequest request,
        ServerCallContext context) =>
        (await ApplyBatch(new BatchRequest
        {
            Mutations =
            {
                new BatchMutation
                {
                    DeleteComponent = request
                }
            }
        }, context)).ToEmpty();

    public override Task<GetSessionResponse> GetSession(ProtoEmpty request,
        ServerCallContext context) =>
        link.GetSessionAsync(context.CancellationToken);

    public override Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context) =>
        link.ImportTextureFileAsync(request, context.CancellationToken);

    public override Task<AssetResponse>
        ImportAudioClipFile(ImportFileRequest request, ServerCallContext context) =>
        link.ImportAudioClipFileAsync(request, context.CancellationToken);

    public override Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context) =>
        link.ImportTextureAsync(request, context.CancellationToken);

    public override Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context) =>
        link.ImportMeshAsync(request, context.CancellationToken);

    public override Task<AssetResponse> ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context)
        => link.ImportAudioClipAsync(request, context.CancellationToken);
}
