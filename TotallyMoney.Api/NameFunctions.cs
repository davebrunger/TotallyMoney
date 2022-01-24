namespace TotallyMoney.Api;

public class NameFunctions
{
    private static readonly string databaseId = Environment.GetEnvironmentVariable("CosmosDbDatabaseId")!;

    [FunctionName("GetNames")]
    [OpenApiOperation(operationId: "GetNames", tags: new[] { "Names" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(NameRecord[]), Description = "The OK response")]
    public async Task<IActionResult> GetNames(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "names")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString", CreateIfNotExists = true)] DocumentClient client,
        ILogger logger)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var names = await client.ToListAsync<NameRecord>(databaseId, NameRecord.CollectionId);

        return new OkObjectResult(names);
    }

    [FunctionName("GetName")]
    [OpenApiOperation(operationId: "GetName", tags: new[] { "Names" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(NameRecord), Description = "The OK response")]
    public async Task<IActionResult> GetName(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "names/{name}")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString", CreateIfNotExists = true)] DocumentClient client,
        ILogger logger,
        string name)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var dbName = await client.SingleOrDefaultAsync<NameRecord>(databaseId, NameRecord.CollectionId, n => n.Name == name);

        if (dbName == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(dbName);
    }

    [FunctionName("SaveName")]
    [OpenApiOperation(operationId: "SaveName", tags: new[] { "Names" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> SaveName(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "names")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString", CreateIfNotExists = true)] DocumentClient client,
        ILogger logger)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        string? name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        if (!string.IsNullOrEmpty(name))
        {
            await client.CreateAsync(databaseId, NameRecord.CollectionId, new NameRecord
            {
                Id = Guid.NewGuid(),
                Name = name,
            });
        }

        string responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        return new OkObjectResult(responseMessage);
    }
}

