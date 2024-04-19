using System.Text.Json.Serialization;

namespace PinballApronCard.Models;
public class Apron
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    public string Manufacturer { get; set; } = default!;
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("type")]
    public string MachineTech { get; set; } = default!;
    [JsonPropertyName("apronCardSize")]
    public string ApronCardSize { get; set; } = default!;
    [JsonPropertyName("qrCode")]
    public string QrCode { get; set; } = default!;
}
