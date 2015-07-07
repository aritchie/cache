using System;
using System.Threading.Tasks;


namespace Acr.Cache.Impl {

    public class VoidCacheImpl : ICache {

        public TimeSpan CleanUpTime { get; set; }
        public TimeSpan DefaultLifeSpan { get; set; }
        public bool Enabled { get; set; }
        public bool Set(string key, object obj, TimeSpan? timeSpan = null) { return false; }
        public T Get<T>(string key) { return default(T); }
        public Task<T> TryGet<T>(string key, Func<Task<T>> getter, TimeSpan? timeSpan = null) { return getter(); }
        public bool Remove(string key) { return false; }
        public void Clear() { }
    }
}
