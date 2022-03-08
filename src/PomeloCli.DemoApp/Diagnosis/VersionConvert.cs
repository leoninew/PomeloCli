using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PomeloCli.DemoApp.Diagnosis {
    class VersionConvert : JsonConverter<Version> {
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var text = reader.GetString();
            if (String.IsNullOrWhiteSpace(text)) {
                return null;
            }
            return Version.Parse(text);
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.ToString());
        }
    }
}