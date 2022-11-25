using System;
using System.Xml.Linq;

namespace HaXMLReader.Interfaces {
    /// <summary>
    /// Is a simple service to create appropriate IReader Instances.
    /// </summary>
    public interface IReaderService {
        IReader RequestReader(XElement element);
        IReader RequestReader(XElement element, Action<string[]> logsink);

        IReader RequestReader(string uri);
        IReader RequestReader(string uri, Action<string[]> logsink);

        IReader RequestStringReader(string toread);
    }
}