using Grpc.Core;
using ResoniteLink;
using Rfmk.Resolink.Grpc.Converters;

namespace Rfmk.Resolink.Grpc;

public class ResolinkService(
    LinkInterface link
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
    
    public override async Task<Slot> GetSlot(GetSlotRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.GetSlotData(new GetSlot
        {
            SlotID = request.SlotId,
            Depth = request.Depth,
            IncludeComponentData = request.IncludeComponentData
        }));
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddSlot(AddSlotRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.AddSlot(new AddSlot
        {
            Data = request.Data.ToModel(),
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateSlot(UpdateSlotRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.UpdateSlot(new UpdateSlot
        {
            Data = request.Data.ToModel(),
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveSlot(DeleteSlotRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.RemoveSlot(new RemoveSlot
        {
            SlotID = request.SlotId,
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Component> GetComponent(GetComponentRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.GetComponentData(new GetComponent
        {
            ComponentID = request.ComponentId,
        }));
        return RaiseMissing(response.Data).ToProto();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> AddComponent(AddComponentRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.AddComponent(new AddComponent
        {
            ContainerSlotId = request.ContainerSlotId,
            Data = request.Data.ToModel(),
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateComponent(UpdateComponentRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.UpdateComponent(new UpdateComponent
        {
            Data = request.Data.ToModel(),
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> RemoveComponent(DeleteComponentRequest request,
        ServerCallContext context)
    {
        RaiseError(await link.RemoveComponent(new RemoveComponent
        {
            ComponentID = request.ComponentId,
        }));
        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<AssetResponse> ImportTextureFile(ImportFileRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.ImportTexture(new ImportTexture2DFile {
            FilePath = request.FilePath,
        }));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClipFile(ImportFileRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.ImportAudioClip(new ImportAudioClipFile {
            FilePath = request.FilePath,
        }));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportTexture(ImportTextureRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.ImportTexture(request.Texture.ToModel()));
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportMesh(ImportMeshRequest request, ServerCallContext context)
    {
        var response = request.MeshKindCase switch
        {
            ImportMeshRequest.MeshKindOneofCase.Json => RaiseError(await link.ImportMesh(request.Json.ToModel())),
            ImportMeshRequest.MeshKindOneofCase.Raw => RaiseError(await link.ImportMesh(request.Raw.ToModel())),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument,
                "An unknown mesh import kind was provided (or one was not provided)."))
        };
        return RaiseMissing(response).ToProto();
    }

    public override async Task<AssetResponse> ImportAudioClip(ImportAudioClipRequest request, ServerCallContext context)
    {
        var response = RaiseError(await link.ImportAudioClip(request.RawClip.ToModel()));
        return RaiseMissing(response).ToProto();
    }
}
