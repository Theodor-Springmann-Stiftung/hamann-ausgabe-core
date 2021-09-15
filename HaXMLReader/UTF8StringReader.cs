using System;
using System.Text;
using System.IO;
using System.Xml;
using HaXMLReader.Interfaces;

namespace HaXMLReader {
    public class UTF8StringReader : Reader, IReader {
        private readonly string _str;
        private StringReader _memoryStream;

        public UTF8StringReader(string str)
        {
            _str = str;
            _memoryStream = new StringReader(str);
            CreateReader();
        }

        public UTF8StringReader(string str, Action<string[]> Logsink) : this(str)
        {
            _LogSink = Logsink;
        }

        public override void Dispose()
        {
            base.Dispose();
            _memoryStream.Dispose();
        }

        protected override XmlReader GetReader() {
            return XmlReader.Create(_memoryStream, _Settings);
        }
    }
}