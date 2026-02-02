namespace Rfmk.Resolink.Grpc;

public interface ILinkConnection
{
    Task<GetSessionResponse> GetSessionAsync(CancellationToken cancellationToken = default);

    Task<BatchResponse> SendBatchAsync(BatchRequest request, CancellationToken cancellationToken = default);

    Task<AssetResponse> ImportTextureFileAsync(ImportFileRequest request,
        CancellationToken cancellationToken = default);

    Task<AssetResponse> ImportAudioClipFileAsync(ImportFileRequest request,
        CancellationToken cancellationToken = default);

    Task<AssetResponse> ImportTextureAsync(ImportTextureRequest request, CancellationToken cancellationToken = default);
    Task<AssetResponse> ImportMeshAsync(ImportMeshRequest request, CancellationToken cancellationToken = default);

    Task<AssetResponse> ImportAudioClipAsync(ImportAudioClipRequest request,
        CancellationToken cancellationToken = default);
}
