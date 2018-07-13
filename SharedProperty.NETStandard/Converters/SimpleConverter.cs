namespace SharedProperty.NETStandard.Converters
{
    public class SimpleConverter : IConverter
    {
        public static readonly SimpleConverter Default = new SimpleConverter();

        public byte[] Convert(byte[] bytes)
        {
            unchecked
            {
                var result = new byte[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    result[i] = ++bytes[i];
                }
                return result;
            }
        }

        public byte[] Deconvert(byte[] bytes)
        {
            unchecked
            {
                var result = new byte[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    result[i] = --bytes[i];
                }
                return result;
            }
        }
    }
}
