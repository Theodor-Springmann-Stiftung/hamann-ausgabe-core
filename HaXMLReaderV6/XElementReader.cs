using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using HaXMLReader.Interfaces;
using System.Xml.Linq;

namespace HaXMLReader
{
    public class XElementReader : Reader, IReader
    {
        private readonly XElement _element;

        public XElementReader(XElement element)
        {
            _element = element;
            CreateReader();
        }

        public XElementReader(XElement element, Action<string[]> Logsink) : this(element)
        {
            _LogSink = Logsink;
        }

        public override void Dispose()
        { 
            base.Dispose();
        }

        protected override XmlReader GetReader() {
            return _element.CreateReader();
        }
        
    }
}
