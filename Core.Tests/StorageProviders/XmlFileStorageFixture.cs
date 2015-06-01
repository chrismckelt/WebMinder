using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.StorageProviders;
using Xunit;

namespace WebMinder.Core.Tests.StorageProviders
{
    public class XmlFileStorageFixture
    {
        private static string _temp = System.IO.Path.GetTempPath();
        private XmlFileStorage<IpAddressRequest> _storage;

        public XmlFileStorageFixture()
        {
            _storage = new XmlFileStorage<IpAddressRequest>();
            _storage.Initialise(new[] { _temp });
            File.Delete(_storage.FileName);
        }

        [Fact]
        public void CreatesFile()
        {
            _storage.SaveStorage();
            Assert.True(File.Exists(_storage.FileName), _storage.FileName);    
        }

        [Fact]
        public void CanPersist()
        {
            var list = new List<IpAddressRequest>
            {
                new IpAddressRequest() {IpAddress = "127.0.0.1", CreatedUtcDateTime = DateTime.MaxValue}
            };
            _storage.Storage = list.AsQueryable();
            _storage.SaveStorage();
            _storage.Initialise(new[] { _temp });

            Assert.Equal(1, _storage.Storage.Count());
            
        }
    }
}