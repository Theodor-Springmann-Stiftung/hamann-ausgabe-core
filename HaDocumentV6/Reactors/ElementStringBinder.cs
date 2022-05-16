using System;
using HaDocument.Models;
using HaXMLReader.EvArgs;
using HaXMLReader.Interfaces;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace HaDocument.Reactors {
    class ElementStringBinder {
        private StringBuilder _stringBuilder;
        private XmlWriter _txtwriter;
        private Stack<Tag> _stack;
        private IReader _reader;
        private Action<string> _callback;

        private bool _normalizeWhitespace = false;

        internal ElementStringBinder(IReader reader, Tag start, Action<string> callback) {
            _reader = reader;
            _callback = callback;
            _stack = new Stack<Tag>();
            _stringBuilder = new StringBuilder();
            _txtwriter = XmlWriter.Create(_stringBuilder, new XmlWriterSettings() { 
                CheckCharacters = false,
                ConformanceLevel = ConformanceLevel.Fragment,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.None
            });
            _stack.Push(start);
            _writeTag(start);
            Subscribe();
        }

        internal ElementStringBinder(IReader reader, Tag start, Action<string> callback, bool normalizeWhitespace) : this(reader, start, callback) {
            _normalizeWhitespace = normalizeWhitespace;
        }

        internal void Subscribe() {
            _reader.Tag += OnTag;
            _reader.Text += OnText;
            _reader.Whitespace += OnWhiteSpace; 
        }

        internal void Unsubscribe() {
            _reader.Tag -= OnTag;
            _reader.Text -= OnText;
            _reader.Whitespace -= OnWhiteSpace;
        }

        internal string GetElementText() {
            _txtwriter.Flush();
            _txtwriter.Dispose();
            Unsubscribe();
            var ret = _stringBuilder.ToString();
            _stringBuilder = new StringBuilder();
            return ret;
        }

        private void _writeTag(Tag tag) {
            var writer = _txtwriter;
            if (tag.EndTag) {
                writer.WriteEndElement();
                return;
            }
            writer.WriteStartElement(tag.Name);
            if (tag.Values != null && tag.Values.Count > 0)
                foreach (var kv in tag.Values)
                    writer.WriteAttributeString(kv.Key, kv.Value);
            if ( tag.IsEmpty ) 
                writer.WriteEndElement();
        }

        private void OnTag(object _, Tag tag) {
            _writeTag(tag);
            if (!tag.IsEmpty && tag.EndTag) _stack.Pop();
            else if (!tag.IsEmpty) _stack.Push(tag);
            if (_stack.Count == 0 ) _callback(GetElementText());
        }

        private void OnText(object _, Text text) {
            if (_normalizeWhitespace) {
                var neu = text.Value.Replace('\n', ' ').Replace("\t", "").Replace("\r", "");
                _txtwriter.WriteString(neu);
            }
            else
                _txtwriter.WriteString(text.Value);
        }

        private void OnWhiteSpace(object _, Whitespace ws) {
            if (_normalizeWhitespace)
                _txtwriter.WriteWhitespace(" ");
            else
                _txtwriter.WriteWhitespace(ws.Value);
        }

    }
}