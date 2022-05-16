using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;

namespace HaXMLReader
{
    public class FileReader : Reader, IReader
    {
        private readonly string _uri;
        private MemoryStream _memoryStream;

        public FileReader(string uri)
        {
            _uri = uri;
            _memoryStream = new MemoryStream(File.ReadAllBytes(_uri));
            CreateReader();
        }

        public FileReader(string uri, Action<string[]> Logsink) : this(uri)
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
