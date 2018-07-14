namespace SharedProperty.NETStandard
{
    public sealed class ConcurrentSharedDictionary : BaseSharedDictionary
    {
        public ConcurrentSharedDictionary(ISerializer serializer, IStorage storage, IConverter converter)
            : base(serializer, storage, converter)
        {
        }

        public override byte[] RawExport()
        {
            SemaphoreSlim.Wait();
            try
            {
                return base.RawExport();
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override void RawImport(byte[] binary)
        {
            SemaphoreSlim.Wait();
            try
            {
                base.RawImport(binary);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override bool ContainsProperty(string key)
        {
            SemaphoreSlim.Wait();
            try
            {
                return base.ContainsProperty(key);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override T GetProperty<T>(string key)
        {
            SemaphoreSlim.Wait();
            try
            {
                return base.GetProperty<T>(key);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override bool TryGetProperty<T>(string key, out T value)
        {
            SemaphoreSlim.Wait();
            try
            {
                return base.TryGetProperty(key, out value);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override void SetProperty<T>(string key, T value)
        {
            SemaphoreSlim.Wait();
            try
            {
                base.SetProperty(key, value);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override bool RemoveProperty(string key)
        {
            SemaphoreSlim.Wait();
            try
            {
                return base.RemoveProperty(key);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public override void ClearProperty()
        {
            SemaphoreSlim.Wait();
            try
            {
                base.ClearProperty();
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
