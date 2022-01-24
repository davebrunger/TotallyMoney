namespace TotallyMoney.Api;

public static class DocumentClientExtensions
{
    public async static Task<List<T>> ToListAsync<T>(this DocumentClient client, string databaseId, string collectionId)
    {
        var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

        var query = client.CreateDocumentQuery<T>(collectionUri).AsDocumentQuery();

        var list = new List<T>();

        while (query.HasMoreResults)
        {
            list.AddRange(await query.ExecuteNextAsync<T>());
        }

        return list;
    }

    public async static Task<T?> SingleOrDefaultAsync<T>(
        this DocumentClient client,
        string databaseId,
        string collectionId,
        Expression<Func<T, bool>>? predicate = null) where T : class
    {
        if (predicate == null)
        {
            predicate = _ => true;
        }

        var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

        var query = client.CreateDocumentQuery<T>(collectionUri)
            .Where(predicate)
            .AsDocumentQuery();

        T? result = null;

        while (query.HasMoreResults)
        {
            if (result != null)
            {
                throw new InvalidOperationException("The input sequence contains more than one element.");
            }
            foreach (var item in await query.ExecuteNextAsync<T>())
            {
                if (result != null)
                {
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                }
                result = item;
                break;
            }
        }

        return result;
    }

    public static Task CreateAsync<T>(
        this DocumentClient client,
        string databaseId,
        string collectionId,
        T document) where T : class
    {
        var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
        return client.CreateDocumentAsync(collectionUri, document);
    }
}

