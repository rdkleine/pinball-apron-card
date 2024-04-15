using System.Text.Json.Serialization;

namespace PinballApronCard.Models;
public class Apron
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("type")]
    public string MachineTech { get; set; } = default!;
    [JsonPropertyName("apronCardSize")]
    public ApronSize Size { get; set; }
    [JsonPropertyName("qrCode")]
    public string QrCode { get; set; } = default!;
}
