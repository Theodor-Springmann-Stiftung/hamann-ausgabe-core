using System.Xml;
using HaXMLReader.Interfaces;

namespace HaXMLReader {
    public class XmlReaderReader : Reader, IReader {
        private XmlReader _initial;

        public XmlReaderReader(XmlReader reader) {
            _initial = reader;
        }

        protected override XmlReader GetReader() {
            return _initial;
        }
    }
}