using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public class SharedDictionary : ISharedDictionary
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ISerializer serializer;
        private readonly IFormatterResolver formatterResolver;
        private readonly IStorage storage;
        private readonly IConverter converter;
        private readonly string filePath;
        private readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();

        public SharedDictionary(ISerializer serializer, IStorage storage, IConverter converter)
        {
            this.serializer = serializer;
            formatterResolver = serializer.FormatterResolver;
            this.storage = storage;
            this.converter = converter;
        }

        public async Task LoadFromStorageAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                properties.Clear();

                if (storage.Exists() == false)
                {
                    return;
                }

                byte[] bytes = await storage.ReadAsync();
                bytes = converter?.Deconvert(bytes) ?? bytes;
                foreach (var property in serializer.Deserialize(bytes))
                {
                    properties[property.Key] = property;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SaveToStorageAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                byte[] bytes = serializer.Serialize(properties.Values);
                bytes = converter?.Convert(bytes) ?? bytes;
                await storage.WriteAsync(bytes);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public byte[] RawExport()
        {
            return serializer.Serialize(properties.Values);
        }

        public void RawImport(byte[] binary)
        {
            properties.Clear();
            foreach (var property in serializer.Deserialize(binary))
            {
                properties[property.Key] = property;
            }
        }

        public bool ContainsKey(string key)
        {
            return properties.ContainsKey(key);
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return ContainsKey(key);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public T GetProperty<T>(string key)
        {
            properties.TryGetValue(key, out Property v);
            if (v is Property<T> property)
            {
                return property.Value;
            }
            else
            {
                throw new KeyNotFoundException($"not found key:{key} or value");
            }
        }

        public async Task<T> GetPropertyAsync<T>(string key)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return GetProperty<T>(key);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public bool TryGetProperty<T>(string key, out T value)
        {
            properties.TryGetValue(key, out Property v);
            if (v is Property<T> property)
            {
                value = property.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public async Task<(bool isSuccess, T value)> TryGetPropertyAsync<T>(string key)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                bool isSuccess = TryGetProperty<T>(key, out T value);
                return (isSuccess, value);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public void SetProperty<T>(string key, T value)
        {
            if (properties.TryGetValue(key, out Property v) && v is Property<T> targetProperty)
            {
                targetProperty.Value = value;
                return;
            }

            var property = new Property<T>
            {
                Key = key,
                Type = TypeCache<T>.FullName,
                Formatter = formatterResolver.Resolve<T>(),
                Value = value
            };
            properties[key] = property;
        }

        public async Task SetPropertyAsync<T>(string key, T value)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                SetProperty(key, value);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public IEnumerator<Property> GetEnumerator()
        {
            return properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}