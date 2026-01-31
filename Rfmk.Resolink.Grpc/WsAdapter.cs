using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Grpc.Core;
using ResoniteLink;

namespace Rfmk.Resolink.Grpc;

public class WsAdapter(ILogger<WsAdapter> logger) : IAsyncDisposable
{
    public bool IsConnected => _client.State == WebSocketState.Open;

    private readonly ClientWebSocket _client = new();
    private readonly AsyncLock _writerLock = new();

    private readonly CancellationTokenSource _cts = new();

    private readonly JsonSerializerOptions _options = new()
    {
        // Necessary for values like Infinity, NaN and so on
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        AllowOutOfOrderMetadataProperties = true,
    };

    private readonly ConcurrentDictionary<string, TaskCompletionSource<Response>>
        _pendingResponses =
            new();

    private Task? _receivingTask;

    private async Task ProcessReceiving()
    {
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await using var reader = WebSocketStream.CreateReadableMessageStream(_client);
                var response = await JsonSerializer.DeserializeAsync<Response>(reader, _options, _cts.Token);

                if (response == null)
                {
                    logger.LogWarning("Received null response from server. Ignoring.");
                    continue;
                }

                logger.LogTrace("Received message {id} to server", response.SourceMessageID);

                if (_pendingResponses.TryRemove(response.SourceMessageID, out var completion))
                {
                    completion.SetResult(response);
                }
                else
                {
                    logger.LogWarning("Received message for unpaired request. Ignoring.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while receiving messages from server.");
        }
    }

    public async Task ConnectAsync(Uri target, CancellationToken cancellationToken)
    {
        using (await _writerLock.LockAsync(cancellationToken))
        {
            if (IsConnected || _cts.IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    "Cannot connect while already connected or the connection canceled.");
            }

            await _client.ConnectAsync(target, cancellationToken);

            _receivingTask = Task.Run(ProcessReceiving, _cts.Token);
        }
    }

    public async Task<Task<Response>> SendAsync(Message message,
        CancellationToken cancellationToken = default)
    {
        message.MessageID ??= Guid.NewGuid().ToString();

        var responseCompletion = new TaskCompletionSource<Response>();
        using (await _writerLock.LockAsync(cancellationToken))
        {
            if (!_pendingResponses.TryAdd(message.MessageID, responseCompletion))
            {
                throw new InvalidOperationException(
                    "Failed to register MessageID. Did you provide duplicate MessageID?");
            }

            logger.LogTrace("Sending message {id} to server", message.MessageID);
            await using (var messageStream =
                         WebSocketStream.CreateWritableMessageStream(_client, WebSocketMessageType.Text))
            {
                await JsonSerializer.SerializeAsync<Message>(messageStream, message, _options, _cts.Token);
            }

            if (message is BinaryPayloadMessage binaryPayload)
            {
                logger.LogTrace("Sending binary message trailer of {size} bytes",
                    binaryPayload.RawBinaryPayload.Length);
                await using var messageStream =
                    WebSocketStream.CreateWritableMessageStream(_client, WebSocketMessageType.Binary);
                await messageStream.WriteAsync(binaryPayload.RawBinaryPayload, _cts.Token);
            }
        }

        return VerifyResponse(responseCompletion.Task);
    }

    private async Task<Response> VerifyResponse(Task<Response> responseTask)
    {
        var response = await responseTask;
        if (!response.Success)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, response.ErrorInfo));
        }

        return response;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();

        await CastAndDispose(_client);
        await CastAndDispose(_writerLock);
        await CastAndDispose(_cts);

        try
        {
            await (_receivingTask ?? Task.CompletedTask);
        }
        catch (OperationCanceledException)
        {
        }

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}
