using SharedProperty.NETStandard;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    interface IUtf8JsonFormatter : IFormatter
    {
        void Write(ref JsonWriter writer, IProperty property);

        IProperty Read(ref JsonReader reader);
    }
}
