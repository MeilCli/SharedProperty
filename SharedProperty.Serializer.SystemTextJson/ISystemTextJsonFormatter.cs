using System.Text.Json;
using SharedProperty.NETStandard;

namespace SharedProperty.Serializer.SystemTextJson
{
    interface ISystemTextJsonFormatter : IFormatter
    {
        void Write(ref Utf8JsonWriter writer, IProperty property);

        IProperty Read(ref Utf8JsonReader reader);
    }
}
