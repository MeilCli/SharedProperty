using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public interface ISharedDictionary : IEnumerable<Property>
    {
        Task LoadFromStorageAsync();

        Task SaveToStorageAsync();

        byte[] RawExport();

        void RawImport(byte[] binary);

        bool ContainsKey(string key);

        Task<bool> ContainsKeyAsync(string key);

        T GetProperty<T>(string key);

        Task<T> GetPropertyAsync<T>(string key);

        bool TryGetProperty<T>(string key, out T value);

        Task<(bool isSuccess, T value)> TryGetPropertyAsync<T>(string key);

        void SetProperty<T>(string key, T value);

        Task SetPropertyAsync<T>(string key, T value);
    }
}
