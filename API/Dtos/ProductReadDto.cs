using System.Text.Json.Serialization;

namespace API.Dtos;

public class ProductReadDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("price")]
    public int Price { get; init; }

    [JsonPropertyName("pictureUrl")]
    public required string PictureUrl { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("brand")]
    public required string Brand { get; init; }
}
