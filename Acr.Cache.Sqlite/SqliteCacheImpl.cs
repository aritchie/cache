using System;
using System.Linq;
using Acr.Cache.Impl;
using Newtonsoft.Json;
using SQLite;


namespace Acr.Cache.Sqlite {

    public class SqliteCacheImpl : AbstractTimerCacheImpl {
        private readonly SQLiteConnection db = new SQLiteConnection("acrcache.db");


        protected override void Init() {
            base.Init();
            this.db.CreateTable<SqlCacheItem>();
        }


        protected override void OnTimerElapsed() {
            var now = DateTime.UtcNow;
            var list = this.db
                .Table<SqlCacheItem>()
                .Where(x => x.DateExpiryUtc < now)
                .ToList();

            foreach (var item in list)
                this.db.Delete(item);
        }


        public override void Clear() {
            this.db.DeleteAll<SqlCacheItem>();
        }


        public override T Get<T>(string key) {
            var item = this.db
                .Table<SqlCacheItem>()
                .FirstOrDefault(x =>
                    x.Key == key &&
                    x.TypeName == typeof(T).FullName
                );

            if (item == null)
                return default(T);

            var obj = JsonConvert.DeserializeObject<T>(item.Json);
            return obj;
        }


        public override bool Remove(string key) {
            var count = this.db.Delete<SqlCacheItem>(key);
            return (count == 1);
        }


        public override void Set(string key, object obj, TimeSpan? timeSpan = null) {
            this.EnsureInitialized();
            var ts = timeSpan ?? this.DefaultLifeSpan;

            var item = this.db.Table<SqlCacheItem>().FirstOrDefault(x => x.Key == key) ?? new SqlCacheItem();
            item.Key = key;
            item.DateExpiryUtc = DateTime.UtcNow.Add(ts);
            item.Json = JsonConvert.SerializeObject(obj);
            item.TypeName = obj.GetType().FullName;
            this.db.InsertOrReplace(item);
        }


        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing)
                this.db.Dispose();
        }
    }
}
