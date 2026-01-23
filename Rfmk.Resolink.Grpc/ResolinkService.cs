using Grpc.Core;
using ResoniteLink;
using Rfmk.Resolink.Grpc.Converters;
using Rfmk.Resolink.Grpc.Link;

namespace Rfmk.Resolink.Grpc;

public class ResolinkService(
    Connection connection
) : LinkService.LinkServiceBase
{
    private static T RaiseError<T>(T response) where T : Response
    {
        if (!response.Success)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, response.ErrorInfo));
        }

        return response;
    }

    private static T RaiseMissing<T>(T val)
    {
        return val ?? throw new RpcException(new Status(StatusCode.InvalidArgument,
            "Did not get a data field back from ResoLink."));
    }

    private async Task<TOutput> InvokeMessageAsync<TInput, TOutput>(
        Func<LinkInterface, TInput, Task<TOutput>> operation,
        TInput input, CancellationToken cancellationToken = default)
        where TOutput : Response
    {
        return RaiseError(await connection.QueueMessageAsync(i => operation(i, input), cancellationToken));
    }

    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.GetSlotData(i), new GetSlot
        {
            SlotID = request.SlotId,
            Depth = request.Depth,
            IncludeComponentData = request.IncludeComponentData
        }, context.CancellationToken);
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddSlot(AddSlotRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.AddSlot(i), new AddSlot
        {
            Data = request.Data.ToModel(),
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateSlot(UpdateSlotRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.UpdateSlot(i), new UpdateSlot
        {
            Data = request.Data.ToModel(),
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveSlot(DeleteSlotRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.RemoveSlot(i), new RemoveSlot
        {
            SlotID = request.SlotId,
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Component> GetComponent(GetComponentRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.GetComponentData(i), new GetComponent
        {
            ComponentID = request.ComponentId,
        });
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddComponent(AddComponentRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.AddComponent(i), new AddComponent
        {
            ContainerSlotId = request.ContainerSlotId,
            Data = request.Data.ToModel(),
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateComponent(UpdateComponentRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.UpdateComponent(i), new UpdateComponent
        {
            Data = request.Data.ToModel(),
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveComponent(DeleteComponentRequest request,
        ServerCallContext context)
    {
        await InvokeMessageAsync((l, i) => l.RemoveComponent(i), new RemoveComponent
        {
            ComponentID = request.ComponentId,
        });
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.ImportTexture(i), new ImportTexture2DFile
        {
            FilePath = request.FilePath,
        });
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClipFile(ImportFileRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.ImportAudioClip(i), new ImportAudioClipFile
        {
            FilePath = request.FilePath,
        });
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.ImportTexture(i), request.Texture.ToModel());
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context)
    {
        var response = request.MeshKindCase switch
        {
            ImportMeshRequest.MeshKindOneofCase.Json => await InvokeMessageAsync((l, i) => l.ImportMesh(i),
                request.Json.ToModel()),
            ImportMeshRequest.MeshKindOneofCase.Raw => await InvokeMessageAsync((l, i) => l.ImportMesh(i),
                request.Raw.ToModel()),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument,
                "An unknown mesh import kind was provided (or one was not provided)."))
        };
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context)
    {
        var response = await InvokeMessageAsync((l, i) => l.ImportAudioClip(i), request.RawClip.ToModel());
        return RaiseMissing(response).ToProto();
    }
}
