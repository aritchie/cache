using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Acr.Cache.Impl {

    public class InMemoryCacheImpl : ICache, IDisposable {
        private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
        private readonly IDictionary<string, object> cache = new Dictionary<string, object>();
        private readonly object syncLock = new object();
        private bool initialized;

        public TimeSpan CleanUpTime { get; set; } = TimeSpan.FromMinutes(2);
        public TimeSpan DefaultLifeSpan { get; set; } = TimeSpan.FromSeconds(30);
        public bool Enabled { get; set; } = true;


        ~InMemoryCacheImpl() {
            this.Dispose(false);
        }


        public void Clear() {
            lock (this.syncLock)
                this.cache.Clear();
        }


        public T Get<T>(string key) {
            if (!this.Enabled)
                return default(T);

            lock (this.syncLock) {
                if (!this.cache.ContainsKey(key))
                    return default(T);

                var item = (CacheItem)this.cache[key];
                return (T)item.Object;
            }
        }


        public bool Remove(string key) {
            if (!this.Enabled)
                return false;

            lock (this.syncLock)
                return this.cache.Remove(key);
        }


        public bool Set(string key, object obj, TimeSpan? timeSpan = null) {
            if (!this.Enabled)
                return false;

            // I only need this call on set, since it doesn't have to clean until there is actually something there
            this.EnsureInit();
            lock (this.syncLock) {
                if (this.cache.ContainsKey(key))
                    return false;

                var ts = timeSpan ?? this.DefaultLifeSpan;
                var cacheObj = new CacheItem {
                    Key = key,
                    Object = obj,
                    ExpiryTime = DateTime.UtcNow.Add(ts)
                };
                this.cache.Add(key, cacheObj);
            }
            return true;
        }


        public async Task<T> TryGet<T>(string key, Func<Task<T>> getter, TimeSpan? timeSpan = null) {
            var obj = this.Get<T>(key);
            if (obj == null) {
                obj = await getter();
                this.Set(key, obj, timeSpan);
            }
            return obj;
        }

        #region Internals

        private void EnsureInit() {
            if (this.initialized)
                return;

            lock (this.syncLock) {
                this.initialized = true;
                this.RunCleanUp();
            }
        }


        private async Task RunCleanUp() {
            while (!this.cancelSource.IsCancellationRequested) {
                try {
                    await Task.Delay(this.CleanUpTime);
                    if (this.cancelSource.IsCancellationRequested)
                        return;

                    var now = DateTime.UtcNow;
                    lock (this.syncLock) {
                        var list = this.cache.Keys
                            .Select(x => (CacheItem) cache[x])
                            .Where(x => x.ExpiryTime < now)
                            .ToList();

                        foreach (var item in list)
                            this.cache.Remove(item.Key);
                    }
                }
                catch (Exception ex) {
                    Debug.WriteLine("[cache cleanup error]: {0}", ex);
                }
            }
        }


        public void Dispose() {
            this.Dispose(true);
        }


        private void Dispose(bool disposing) {
            if (!disposing)
                return;

            this.cancelSource.Cancel(false);
        }

        #endregion
    }
}