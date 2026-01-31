using ResoniteLink;
using Rfmk.Resolink.Grpc.Converters;

namespace Rfmk.Resolink.Grpc;

public class BatchAdaptor(WsAdapter link)
{
    public async Task<BatchResponse> SendBatch(BatchRequest request, CancellationToken cancellationToken = default)
    {
        List<Task<Response>> mutations = [];
        foreach (var mut in request.Mutations)
        {
            var mutMessage = mut.ToModel();
            if (mutMessage != null)
            {
                mutations.Add(await link.SendAsync(mutMessage, cancellationToken));
            }
        }

        List<Task<Response>> queries = [];
        foreach (var query in request.Queries)
        {
            var queryMessage = query.ToModel();
            if (queryMessage != null)
            {
                queries.Add(await link.SendAsync(queryMessage, cancellationToken));
            }
        }

        await Task.WhenAll(mutations);
        var queryResponses = await Task.WhenAll(queries);

        return new BatchResponse
        {
            Queries = { queryResponses.Select(r => r.ToProto()) }
        };
    }
}