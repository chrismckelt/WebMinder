using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WebMinder.Core.StorageProviders
{
    public class XmlFileStorageProvider<T> : IStorageProvider<T>
    {
        private const string RootXml = "WebMinder";

        private static readonly XmlSerializerNamespaces Namespaces =
            new XmlSerializerNamespaces(new[] {new XmlQualifiedName("", "")});

        private XDocument _dataFile;
        public string FileName { get; private set; }

        public void Initialise(string[] args)
        {
            var folder = args.First();
            FileName = Path.Combine(folder, RootXml + ".resource");
            if (!File.Exists(FileName))
            {
                _dataFile = XDocument.Parse("<WebMinder></WebMinder>");
                _dataFile.Save(FileName);
            }
            _dataFile = XDocument.Load(FileName);

            //Run query
            var posts = from lv1 in _dataFile.Descendants(RootXml)
                select lv1;

            if (Items == null) Items = new EnumerableQuery<T>(new List<T>());
            var items = new List<T>();
            posts.Where(x => x.HasElements)
                .ToList()
                .ForEach(x => items.Add(Deserialize(x.Element(typeof (T).Name).Value)));
            Items = items.AsQueryable();
        }

        public IQueryable<T> Items { get; set; }

        public void SaveStorage()
        {
            //_dataFile.Element(RootXml).Add(els);
            foreach (var sss in Items)
            {
                _dataFile.Element(RootXml).Add(new XElement(typeof(T).Name, Serialize(sss)));
            }
            
            _dataFile.Save(FileName);
        }

        public static T Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof (T));
            using (var reader = new StringReader(xml))
            {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static string Serialize(object obj)
        {
            var serializer = new XmlSerializer(typeof (T));
            var xw = new StringWriter();
            serializer.Serialize(xw, obj, Namespaces);
            return xw.ToString();
        }
    }
}