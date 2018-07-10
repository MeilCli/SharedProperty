using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public interface IStorage
    {
        bool Exists();

        Task<byte[]> ReadAsync();

        Task WriteAsync(byte[] bytes);
    }
}
