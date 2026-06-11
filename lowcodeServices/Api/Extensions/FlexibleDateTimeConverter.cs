using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Extensions;

/// <summary>
/// 支持 ISO 字符串与毫秒时间戳的 DateTime 反序列化.
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var ms))
                    return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
                break;
            case JsonTokenType.String:
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s, out var dt)) return dt;
                break;
        }

        throw new JsonException($"无法将 {reader.TokenType} 转换为 DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        else
            writer.WriteNullValue();
    }
}

public class FlexibleDateTimeNonNullableConverter : JsonConverter<DateTime>
{
    private readonly FlexibleDateTimeConverter _inner = new();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => _inner.Read(ref reader, typeof(DateTime?), options) ?? default;

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => _inner.Write(writer, value, options);
}
