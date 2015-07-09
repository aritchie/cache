using System;
using System.Threading.Tasks;


namespace Acr.Cache.Impl {

    public abstract class AbstractCacheImpl : ICache, IDisposable {
        private readonly object syncLock = new object();
        private bool init;


        public virtual TimeSpan CleanUpTime { get; set; } = TimeSpan.FromSeconds(20);
        public virtual TimeSpan DefaultLifeSpan { get; set; } = TimeSpan.FromMinutes(2);
        public virtual bool Enabled { get; set; } = true;

        public abstract void Clear();

        public abstract T Get<T>(string key);
        public abstract bool Remove(string key);
        public abstract void Set(string key, object obj, TimeSpan? timeSpan = default(TimeSpan?));
        protected abstract void Init();

        protected void EnsureInitialized() {
            if (this.init)
                return;

            lock (this.syncLock) {
                this.Init();
                this.init = true;
            }
        }


        public virtual async Task<T> TryGet<T>(string key, Func<Task<T>> getter, TimeSpan? timeSpan = default(TimeSpan?)) {
            var obj = this.Get<T>(key);
            if (obj == null) {
                obj = await getter();
                this.Set(key, obj, timeSpan);
            }
            return obj;
        }


        protected virtual void Dispose(bool disposing) {
        }


        public virtual void Dispose() {
            this.Dispose(true);
        }
    }
}
