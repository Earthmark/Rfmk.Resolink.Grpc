using Grpc.Core;
using Rfmk.Resolink.Grpc.Converters;
using Rfmk.Resolink.Grpc.Link;

namespace Rfmk.Resolink.Grpc;

public class ResolinkService(
    Connection connection
) : LinkService.LinkServiceBase
{
    private static T RaiseMissing<T>(T val)
    {
        return val ?? throw new RpcException(new Status(StatusCode.InvalidArgument,
            "Did not get a data field back from ResoLink."));
    }

    public override async Task<GetSessionResponse> GetSession(Google.Protobuf.WellKnownTypes.Empty request,
        ServerCallContext context)
    {
        var response =
            await connection.QueueMessageAsync((l) => l.GetSessionData(), context.CancellationToken);
        return response.ToProto();
    }

    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context)
    {
        var req = request.ToModel();
        var response = await connection.QueueMessageAsync(l => l.GetSlotData(req), context.CancellationToken);
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddSlot(AddSlotRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.AddSlot(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateSlot(UpdateSlotRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.UpdateSlot(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveSlot(DeleteSlotRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.RemoveSlot(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Component> GetComponent(GetComponentRequest request, ServerCallContext context)
    {
        var req = request.ToModel();
        var response = await connection.QueueMessageAsync(l => l.GetComponentData(req));
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddComponent(AddComponentRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.AddComponent(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateComponent(UpdateComponentRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.UpdateComponent(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveComponent(DeleteComponentRequest request,
        ServerCallContext context)
    {
        var req = request.ToModel();
        await connection.QueueMessageAsync(l => l.RemoveComponent(req));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context)
    {
        var req = request.ToModelTexture();
        var response = await connection.QueueMessageAsync(l => l.ImportTexture(req));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClipFile(ImportFileRequest request, ServerCallContext context)
    {
        var req = request.ToModelAudio();
        var response = await connection.QueueMessageAsync(l => l.ImportAudioClip(req));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context)
    {
        var req = request.Texture.ToModel();
        var response = await connection.QueueMessageAsync(l => l.ImportTexture(req));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context)
    {
        var response = request.MeshKindCase switch
        {
            ImportMeshRequest.MeshKindOneofCase.Json =>
                await connection.QueueMessageAsync(l => l.ImportMesh(request.Json.ToModel())),
            ImportMeshRequest.MeshKindOneofCase.Raw => await connection.QueueMessageAsync(l =>
                l.ImportMesh(request.Raw.ToModel())),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument,
                "An unknown mesh import kind was provided (or one was not provided)."))
        };
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context)
    {
        var req = request.RawClip.ToModel();
        var response = await connection.QueueMessageAsync(l => l.ImportAudioClip(req));
        return RaiseMissing(response).ToProto();
    }
}
