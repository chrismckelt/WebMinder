using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebMinder.Core.Rules.IpBlocker;
using WebMinder.Core.StorageProviders;
using Xunit;

namespace WebMinder.Core.Tests.StorageProviders
{
    public class XmlFileStorageProviderFixture
    {
        private static string _temp = System.IO.Path.GetTempPath();
        private XmlFileStorageProvider<IpAddressRequest> _storageProvider;

        public XmlFileStorageProviderFixture()
        {
            _storageProvider = new XmlFileStorageProvider<IpAddressRequest>();
            _storageProvider.Initialise(new[] { _temp });
            File.Delete(_storageProvider.FileName);
        }

        [Fact]
        public void CreatesFile()
        {
            _storageProvider.SaveStorage();
            Assert.True(File.Exists(_storageProvider.FileName), _storageProvider.FileName);    
        }

        [Fact]
        public void CanPersist()
        {
            var list = new List<IpAddressRequest>
            {
                new IpAddressRequest() {IpAddress = "127.0.0.1", CreatedUtcDateTime = DateTime.MaxValue}
            };
            _storageProvider.Items = list.AsQueryable();
            _storageProvider.SaveStorage();
            _storageProvider.Initialise(new[] { _temp });

            Assert.Equal(1, _storageProvider.Items.Count());
            
        }
    }
}