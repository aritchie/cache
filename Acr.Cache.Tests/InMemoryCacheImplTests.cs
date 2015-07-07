using System;
using System.Threading.Tasks;
using Acr.Cache.Impl;
using NUnit.Framework;


namespace Acr.Cache.Tests {

    public class InMemoryCacheImplTests {
        private InMemoryCacheImpl cache;


        [SetUp]
        public void OnBeforeTest() {
            this.cache = new InMemoryCacheImpl();
        }


        [TearDown]
        public void OnAfterTest() {
            this.cache.Dispose();
        }


        [Test]
        public void BasicTest() {
            var dt = DateTime.Now;
            this.cache.Set("BasicTest", dt);
            var get = this.cache.Get<DateTime>("BasicTest");
            Assert.AreEqual(dt, get);
        }


        [Test]
        public void RemoveTest() {
            this.cache.Set("RemoveTest", new object());
            this.cache.Remove("RemoveTest");
            var obj = this.cache.Get<object>("RemoveTest");
            Assert.IsNull(obj);
        }


        [Test]
        public async Task CleanUpTest() {
            this.cache.CleanUpTime = TimeSpan.FromSeconds(1);
            this.cache.Set("CleanUpTest", new object(), TimeSpan.FromSeconds(1));
            await Task.Delay(3000);
            var obj = this.cache.Get<object>("CleanUpTest");
            Assert.IsNull(obj);
        }
    }
}
