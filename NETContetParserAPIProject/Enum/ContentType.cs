using System.Text.Json.Serialization;

namespace Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContentType
{
    CSV,
    INTERNAL_JSON
}