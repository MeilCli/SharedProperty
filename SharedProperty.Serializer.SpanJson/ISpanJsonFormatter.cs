using SharedProperty.NETStandard;
using SpanJson;

namespace SharedProperty.Serializer.SpanJson
{
    interface ISpanJsonFormatter : IFormatter
    {
        void Write(ref JsonWriter<byte> writer, IProperty property);

        IProperty Read(ref JsonReader<byte> reader);
    }
}
