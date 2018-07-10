using System.Collections.Generic;

namespace SharedProperty.NETStandard
{
    public interface ISerializer
    {
        IFormatterResolver FormatterResolver { get; }

        byte[] Serialize(IEnumerable<Property> properties);

        IEnumerable<Property> Deserialize(byte[] binary);
    }
}
