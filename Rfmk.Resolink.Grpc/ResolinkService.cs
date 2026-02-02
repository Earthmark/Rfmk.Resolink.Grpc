using Grpc.Core;
using Microsoft.Extensions.Options;
using Rfmk.Resolink.Grpc.Projectors;
using ProtoEmpty = Google.Protobuf.WellKnownTypes.Empty;

namespace Rfmk.Resolink.Grpc;

public class ResolinkService(
    IBatchProjector projector,
    ILinkConnection link,
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    IOptionsSnapshot<LinkServiceOptions> opts
) : LinkService.LinkServiceBase
{
    public override async Task<BatchResponse> ApplyBatch(BatchRequest request, ServerCallContext context)
    {
        projector.PrepareRequest(request);
        var response = await link.SendBatchAsync(request, context.CancellationToken);
        projector.PrepareResponse(request, response);
        return response;
    }

    private static BatchQueryResponse GetFirstResponse(BatchResponse response)
        => response.Queries.FirstOrDefault() ??
           throw new RpcException(
               new Status(StatusCode.Internal,
                   "Expected a query response, but non was provided."));

    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context) =>
        GetFirstResponse(await ApplyBatch(new BatchRequest
        {
            Queries =
            {
                new BatchQuery
                {
                    GetSlot = request
                }
            }
        }, context)).Slot;

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
        GetFirstResponse(await ApplyBatch(new BatchRequest
        {
            Queries =
            {
                new BatchQuery
                {
                    GetComponent = request
                }
            }
        }, context)).Component;

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

    private void CheckFileOpsAllowed()
    {
        if (!opts.Value.AllowFileOps)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "File operations are disabled, this is configured in API settings."));
        }
    }

    public override Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context)
    {
        CheckFileOpsAllowed();
        return link.ImportTextureFileAsync(request, context.CancellationToken);
    }

    public override Task<AssetResponse>
        ImportAudioClipFile(ImportFileRequest request, ServerCallContext context)
    {
        CheckFileOpsAllowed();
        return link.ImportAudioClipFileAsync(request, context.CancellationToken);
    }

    public override Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context) =>
        link.ImportTextureAsync(request, context.CancellationToken);

    public override Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context) =>
        link.ImportMeshAsync(request, context.CancellationToken);

    public override Task<AssetResponse> ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context)
        => link.ImportAudioClipAsync(request, context.CancellationToken);
}
