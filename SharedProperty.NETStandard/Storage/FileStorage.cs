using System;
using System.IO;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard.Storage
{
    public class FileStorage : IStorage
    {
        private static readonly string defaultFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}shared_property.json";
        public static readonly FileStorage Default = new FileStorage();

        private readonly string filePath;

        public FileStorage() : this(defaultFilePath) { }

        public FileStorage(string filePath)
        {
            this.filePath = filePath;
        }

        public bool Exists()
        {
            return File.Exists(filePath);
        }

        public async Task<byte[]> ReadAsync()
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public async Task WriteAsync(byte[] bytes)
        {
            using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
