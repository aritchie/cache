using System;
using SQLite.Net.Attributes;


namespace Acr.Cache.Sqlite {

    public class SqlCacheItem {

        [PrimaryKey]
        public string Key { get; set; }
        public DateTime DateExpiryUtc { get; set; }
        public string TypeName { get; set; }
        public string Json { get; set; }
    }
}
