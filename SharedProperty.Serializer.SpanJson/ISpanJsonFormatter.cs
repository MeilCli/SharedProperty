using SharedProperty.NETStandard;
using SpanJson;

namespace SharedProperty.Serializer.SpanJson
{
    interface ISpanJsonFormatter : IFormatter
    {
        void Write(ref JsonWriter<byte> writer, Property property);

        Property Read(ref JsonReader<byte> reader);
    }
}
