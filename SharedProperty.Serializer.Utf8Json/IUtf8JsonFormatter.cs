using SharedProperty.NETStandard;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    interface IUtf8JsonFormatter : IFormatter
    {
        void Write(ref JsonWriter writer, Property property);

        Property Read(ref JsonReader reader);
    }
}
