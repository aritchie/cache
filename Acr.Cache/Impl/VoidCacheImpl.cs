using System;
using System.Threading.Tasks;


namespace Acr.Cache.Impl {

    public class VoidCacheImpl : AbstractCacheImpl {
        protected override void Init() {}
        public override T Get<T>(string key) { return default(T); }
        public override void Set(string key, object obj, TimeSpan? timeSpan = null) {}
        public override void Clear() {}
        public override bool Remove(string key) { return false; }
    }
}
