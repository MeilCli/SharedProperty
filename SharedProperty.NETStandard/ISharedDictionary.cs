using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public interface ISharedDictionary : IEnumerable<IProperty>
    {
        Task LoadFromStorageAsync();

        Task SaveToStorageAsync();

        byte[] RawExport();

        void RawImport(byte[] binary);

        bool ContainsKey(string key);

        T GetProperty<T>(string key);

        bool TryGetProperty<T>(string key, out T value);

        void SetProperty<T>(string key, T value);

        bool RemoveProperty(string key);
    }
}
