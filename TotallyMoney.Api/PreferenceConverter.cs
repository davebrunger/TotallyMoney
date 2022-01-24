namespace TotallyMoney.Api;

public class PreferenceConverter : JsonConverter<OneOf<int, DayOfWeek[], bool>>
{
    public override OneOf<int, DayOfWeek[], bool> ReadJson(
        JsonReader reader, 
        Type objectType, 
        OneOf<int, DayOfWeek[], bool> existingValue, 
        bool hasExistingValue, 
        JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject
            || !reader.Read()
            || reader.TokenType != JsonToken.PropertyName
            || reader.Value?.ToString() !="type"
            || !reader.Read()
            || reader.TokenType != JsonToken.String)
        {
            throw new JsonSerializationException();
        }
        var result = (string)reader.Value switch
        {
            "specificDate" => ReadSpecificDate(reader),
            "daysOfWeek" => ReadDaysOfWeek(reader),
            "everyDay" => true,
            "never" => false,
            _ => throw new JsonSerializationException()
        };
        if (!reader.Read()
            || reader.TokenType != JsonToken.EndObject)
        {
            throw new JsonSerializationException();
        }
        return result;
    }

    private OneOf<int, DayOfWeek[], bool> ReadSpecificDate(JsonReader reader)
    {
        if (!reader.Read()
            || reader.TokenType != JsonToken.PropertyName
            || reader.Value?.ToString() != "specificDate"
            || !reader.Read()
            || reader.TokenType != JsonToken.Integer)
        {
            throw new JsonSerializationException();
        }
        return (int)(long)reader.Value;
    }

    private OneOf<int, DayOfWeek[], bool> ReadDaysOfWeek(JsonReader reader)
    {
        if (!reader.Read()
            || reader.TokenType != JsonToken.PropertyName
            || reader.Value?.ToString() != "daysOfWeek"
            || !reader.Read()
            || reader.TokenType != JsonToken.StartArray)
        {
            throw new JsonSerializationException();
        }

        var days = new List<DayOfWeek>();

        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
        {
            if (reader.TokenType != JsonToken.String)
            {
                days.Add(Enum.Parse<DayOfWeek>((string)reader.Value, true));
            }
        }

        return days.ToArray();
    }

    public override void WriteJson(JsonWriter writer, OneOf<int, DayOfWeek[], bool> value, JsonSerializer serializer)
    {
        value.Switch(
            specificDate => serializer.Serialize(writer, new { type = "specificDate", specificDate = specificDate }),
            daysOfWeek => serializer.Serialize(writer, new { type = "daysOfWeek", daysOfWeek = daysOfWeek.Select(d => d.ToString()).ToArray() }),
            everyDay => serializer.Serialize(writer, new { type = everyDay ? "everyDay" : "never" })
        );
    }
}