using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Anemone.Repository.HeatingSystem;

public class PersistenceHeatingSystemConverter : JsonConverter<PersistenceHeatingSystemModel>
{
    // do not change propertyNames, might break deserialization of old files
    private const string NamePropertyName = "Name";
    private const string FrequencyPropertyName = "Frequency";
    private const string TemperaturePropertyName = "Temperature";


    public override PersistenceHeatingSystemModel Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();


        var name = string.Empty;
        List<HeatingSystemDataPointModel> frequencyData = new();
        List<HeatingSystemDataPointModel> temperatureData = new();

        do
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            var propertyName = reader.GetString();
            reader.Read();
            switch (propertyName)
            {
                case NamePropertyName:
                    name = reader.GetString() ?? throw new JsonException();
                    break;
                case FrequencyPropertyName:
                    ReadHeatingSystemDataCollectionItems(ref reader, frequencyData);
                    break;
                case TemperaturePropertyName:
                    ReadHeatingSystemDataCollectionItems(ref reader, temperatureData);
                    break;
            }
        } while (reader.Read());

        return new PersistenceHeatingSystemModel
        {
            Name = name,
            FrequencyData = frequencyData,
            TemperatureData = temperatureData
        };
    }

    public override void Write(Utf8JsonWriter writer, PersistenceHeatingSystemModel value,
        JsonSerializerOptions options)
    {
        var sortedFrequency = value.FrequencyData.OrderBy(x => x.Key).ToList();
        var sortedTemperature = value.TemperatureData.OrderBy(x => x.Key).ToList();

        writer.WriteStartObject();


        writer.WriteString(NamePropertyName, value.Name);


        writer.WriteStartArray(FrequencyPropertyName);
        WriteHeatingSystemDataCollectionItems(writer, sortedFrequency);
        writer.WriteEndArray();


        writer.WriteStartArray(TemperaturePropertyName);
        WriteHeatingSystemDataCollectionItems(writer, sortedTemperature);
        writer.WriteEndArray();


        writer.WriteEndObject();
    }

    private static void ReadHeatingSystemDataCollectionItems(ref Utf8JsonReader reader,
        ICollection<HeatingSystemDataPointModel> data)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException();
            var key = reader.GetDouble();

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException();
            var resistance = reader.GetDouble();

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException();
            var inductance = reader.GetDouble();

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            data.Add(new HeatingSystemDataPointModel(key, resistance, inductance));
            reader.Read();
        }
    }

    private static void WriteHeatingSystemDataCollectionItems(Utf8JsonWriter writer,
        List<HeatingSystemDataPointModel> data)
    {
        foreach (var hs in data)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(hs.Key);
            writer.WriteNumberValue(hs.Resistance);
            writer.WriteNumberValue(hs.Inductance);
            writer.WriteEndArray();
        }
    }
}