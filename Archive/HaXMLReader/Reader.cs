using System;
using HaXMLReader.EvArgs;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using HaXMLReader.Interfaces;

namespace HaXMLReader
{
    public abstract class Reader : IReader
    {
        public event EventHandler ReadingStart;
        public event EventHandler ReadingStop;
        public event EventHandler<Tag> Tag;
        public event EventHandler<Tag> OpenTag;
        public event EventHandler<Tag> CloseTag;
        public event EventHandler<Tag> SingleTag;
        public event EventHandler<Whitespace> Whitespace;
        public event EventHandler<Text> Text;

        protected Action<string[]> _LogSink { get; set; }
        private XmlReader _XReader { get; set; } = null;

        //State:
        internal XmlReaderSettings _Settings = new XmlReaderSettings()
        {
            CloseInput = true,
            CheckCharacters = false,
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = false
        };

        public void Log(params string[] message)
        {
            if (_LogSink != null) _LogSink.Invoke(message.Select(x => x = "HamannDocument: \t" + x).ToArray());
        }

        public void Read()
        {
            if (_XReader == null || 
                _XReader.ReadState == ReadState.Closed || 
                _XReader.ReadState == ReadState.EndOfFile ||
                _XReader.ReadState == ReadState.Error) {
                    CreateReader();
            }
            ReadingStart?.Invoke(this, EventArgs.Empty);
            while (_XReader.Read())
            {
                _Read();
            }
            ReadingStop?.Invoke(this, EventArgs.Empty);
            Dispose();
        }

        private void _Read() {
            try {
                switch (_XReader.NodeType)
                {
                    case XmlNodeType.Text:
                        Text?.Invoke(this, new Text() { Value = _XReader.Value });
                        break;
                    case XmlNodeType.SignificantWhitespace:
                        Whitespace?.Invoke(this, new Whitespace() { Value = _XReader.Value });
                        break;
                    case XmlNodeType.Whitespace:
                        Whitespace?.Invoke(this, new Whitespace() { Value = _XReader.Value });
                        break;
                    case XmlNodeType.Element:
                        var tag = _TagCreation();
                        Tag?.Invoke(this, tag);
                        if (tag.IsEmpty) SingleTag?.Invoke(this, tag);
                        else OpenTag?.Invoke(this, tag);
                        break;
                    case XmlNodeType.EndElement:
                        var tg = _TagCreation();
                        Tag?.Invoke(this, tg);
                        CloseTag?.Invoke(this, tg);
                        break;
                }
            }
            catch (XmlException ex) {
                Log(ex.Message);
                throw ex;
            }
        }

        public IReader CreateReaderForSubtree(string elementname) {
            if (_XReader == null) CreateReader();
            else throw new InvalidOperationException("Der Status des Readers kann nicht verändert werden.");
            _skipUntilElement(elementname);
            return _createReaderForSubtree();
        }

        public IReader CreateReaderForSubtree(Func<Tag, bool> Condition) {
            if (_XReader == null) CreateReader();
            else throw new InvalidOperationException("Der Status des Readers kann nicht verändert werden.");
            while (_XReader.Read()) {
                if(_XReader.NodeType == XmlNodeType.Element) {
                    var tag = _TagCreation();
                    if (Condition(tag))
                        return _createReaderForSubtree();
                }
            }
            return null;
        }

        private IReader _createReaderForSubtree() {
            if (_XReader.NodeType == XmlNodeType.Attribute)
                _XReader.MoveToElement();
            return new XmlReaderReader(_XReader.ReadSubtree());
        }

        public void ImportSettings(XmlReaderSettings settings) {
            _Settings = settings;
        }

        public virtual void Dispose() {
        }

        // Legacy:
        public void Close() {
            _XReader.Close();
        }

        protected void CreateReader() {
            if (_XReader == null || 
                _XReader.ReadState == ReadState.Closed || 
                _XReader.ReadState == ReadState.EndOfFile ||
                _XReader.ReadState == ReadState.Error) {
                _XReader = GetReader();
            }
        }
        
        protected abstract XmlReader GetReader();

        private void _skipUntilElement(string name) {
            if (_XReader == null) CreateReader();
            else throw new InvalidOperationException("Der Status des Readers kann nicht verändert werden.");
            _XReader.ReadToFollowing(name);
        }

        private Tag _TagCreation() {
            var tag = new Tag();
            tag.Name = _XReader.Name;
            tag.IsEmpty = _XReader.IsEmptyElement;
            tag.EndTag = _XReader.NodeType == XmlNodeType.EndElement ? true : false;
            if (_XReader.HasAttributes)
            {                
                int no = _XReader.AttributeCount;
                for (int step = 0; step < no; step++)
                {
                    _XReader.MoveToAttribute(step);
                    tag.Values.Add(_XReader.Name.ToLower(), _XReader.Value);
                }
            }
            return tag;
        }

        private void _checkReadStateInteractive() {
            if (_XReader != null && _XReader.ReadState != ReadState.Interactive) {
                Dispose();
                var ex = new InvalidOperationException("The Reader is currently not in a reading state");
                Log(ex.Message);
                throw ex;
            }
            return;
        }
    }
}
