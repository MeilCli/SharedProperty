using System.Collections.Generic;

namespace SharedProperty.NETStandard
{
    public interface ISerializer
    {
        IFormatterResolver FormatterResolver { get; }

        byte[] Serialize(IEnumerable<IProperty> properties);

        IEnumerable<IProperty> Deserialize(byte[] binary);
    }
}
