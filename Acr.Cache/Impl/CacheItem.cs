using System;


namespace Acr.Cache.Impl {

    public class CacheItem {

        public string Key { get; set; }
        public DateTime ExpiryTime { get; set; }
        public object Object { get; set; }
    }
}
