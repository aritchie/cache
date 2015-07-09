using System;
using System.IO;
using Acr.Cache.Sqlite;
using NUnit.Framework;


namespace Acr.Cache.Tests {

    [TestFixture]
    public class SqliteCacheImplTests : ProviderTestFixture<SqliteCacheImpl> {

        [TestFixtureSetUp]
        public void OnBeforeTests() {
            if (File.Exists("acrcache.db3"))
                File.Delete("acrcache.db3");
        }
    }
}
