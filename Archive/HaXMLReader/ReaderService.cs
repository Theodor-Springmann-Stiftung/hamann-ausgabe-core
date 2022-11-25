using System;
using System.Xml.Linq;
using HaXMLReader.Interfaces;

namespace HaXMLReader {
    /// <summary>
    /// Very basic IReader-Factory. Lifetime of a reader must be handled outside.
    /// Allows using any Reader as a service e.g. within web projects.
    /// Allow injecting the service into another project.
    /// </summary>
    public class ReaderService : IReaderService {
        public IReader RequestReader(XElement element) => new XElementReader(element);
        public IReader RequestReader(XElement element, Action<string[]> logsink) => new XElementReader(element, logsink);
        public IReader RequestReader(string uri) => new FileReader(uri);
        public IReader RequestReader(string uri, Action<string[]> logsink) => new FileReader(uri, logsink);
        public IReader RequestStringReader(String toread) => new UTF8StringReader(toread);
    }
}