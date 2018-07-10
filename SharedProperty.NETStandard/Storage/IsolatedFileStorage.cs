using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard.Storage
{
    public class IsolatedFileStorage : IStorage
    {
        private const string defaultFileName = "shared_property.json";
        public static readonly IsolatedFileStorage Default = new IsolatedFileStorage();

        private readonly string fileName;
        private readonly IsolatedStorageFile isolatedStorageFile;

        public IsolatedFileStorage() : this(defaultFileName) { }

        public IsolatedFileStorage(string fileName)
        {
            this.fileName = fileName;
            isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public bool Exists()
        {
            return isolatedStorageFile.FileExists(fileName);
        }

        public async Task<byte[]> ReadAsync()
        {
            using (var fileStream = isolatedStorageFile.OpenFile(fileName, FileMode.Open))
            {
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public async Task WriteAsync(byte[] bytes)
        {
            using (var fileStream = isolatedStorageFile.OpenFile(fileName, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
