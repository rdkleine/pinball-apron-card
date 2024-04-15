using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PinballApronCard.Models;

namespace PinballApronCard.Helpers;

public class ApronSizeConverter : JsonConverter<ApronSize>
{
    public override ApronSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        switch (stringValue)
        {
            case "38x96-em-bally":
                return ApronSize.EmBally;
            case "54x98-em-wms":
                return ApronSize.EmWms;
            case "57x104-zaccaria":
                return ApronSize.Zaccaria;
            case "57x154-gottlieb":
                return ApronSize.Gottlieb;
            case "75x138mm-sega":
                return ApronSize.Sega;
            case "75x140-stern-modern":
                return ApronSize.SternModern;
            case "76x140-data-east-game-plan":
                return ApronSize.DataEastGamePlan;
            case "77x140-stern-classic":
                return ApronSize.SternClassic;
            case "83x140-ss-bally":
                return ApronSize.SsBally;
            case "83x143-capcom":
                return ApronSize.CapCom;
            case "83x154-wms-jjp":
                return ApronSize.WmsJjp;
            default:
                if (stringValue == "skipped" || string.IsNullOrEmpty(stringValue))
                {
                    return ApronSize.Skipped;
                }
                throw new JsonException($"Value '{stringValue}' is not recognized as a valid ApronSize.");
        }
    }

    public override void Write(Utf8JsonWriter writer, ApronSize value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case ApronSize.EmBally:
                writer.WriteStringValue("38x96-em-bally");
                break;
            case ApronSize.EmWms:
                writer.WriteStringValue("54x98-em-wms");
                break;
            // Add cases for all enum values
            default:
                writer.WriteStringValue("skipped");
                break;
        }
    }
}
