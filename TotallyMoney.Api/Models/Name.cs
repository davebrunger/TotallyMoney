namespace TotallyMoney.Api.Models;

public class NameRecord
{
    public static string CollectionId = "names";
    
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = null!;
}

