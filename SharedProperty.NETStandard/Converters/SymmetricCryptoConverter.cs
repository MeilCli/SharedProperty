using System.Security.Cryptography;

namespace SharedProperty.NETStandard.Converters
{
    public class SymmetricCryptoConverter : IConverter
    {
        protected static readonly byte[] Default256BitKey = new byte[] {
            238, 18, 238, 135, 153,
            155, 135, 225, 232, 166,
            245, 35, 243, 15, 113,
            55, 125, 222, 234, 178,
            143, 240, 138, 191, 147,
            30, 148, 15, 223, 55, 11, 244
        };

        protected static readonly byte[] Default128BitIv = new byte[] {
            248, 117, 4, 142, 146,
            121, 108, 56, 123, 215,
            207, 207, 126, 188, 183, 66
        };

        private readonly SymmetricAlgorithm symmetricAlgorithm;

        public SymmetricCryptoConverter(SymmetricAlgorithm symmetricAlgorithm)
        {
            this.symmetricAlgorithm = symmetricAlgorithm;
        }

        public byte[] Convert(byte[] bytes)
        {
            return symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public byte[] Deconvert(byte[] bytes)
        {
            return symmetricAlgorithm.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length);
        }
    }
}
