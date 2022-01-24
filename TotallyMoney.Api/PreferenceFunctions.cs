namespace TotallyMoney.Api;

internal class PreferenceFunctions
{
    private static readonly string databaseId = Environment.GetEnvironmentVariable("CosmosDbDatabaseId")!;

    [FunctionName("SavePreference")]
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

