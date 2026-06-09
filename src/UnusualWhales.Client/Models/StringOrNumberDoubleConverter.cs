using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnusualWhales.Client.Models;

/// <summary>
/// Reads a JSON <see cref="double"/> from either a number token or a string
/// token. The Unusual Whales API is inconsistent: some numeric fields come
/// back quoted (e.g. <c>"correlation": "0.88..."</c>) and the default
/// <see cref="System.Text.Json"/> behaviour throws on the quoted form.
/// </summary>
internal sealed class StringOrNumberDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDouble();

        if (reader.TokenType == JsonTokenType.String)
        {
            var raw = reader.GetString();
            if (string.IsNullOrEmpty(raw))
                return 0d;

            return double.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing double.");
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value);
}
