using System;
using System.Xml;
using HaXMLReader.EvArgs;

namespace HaXMLReader.Interfaces {
    public interface IReader {
        event EventHandler ReadingStart;
        event EventHandler ReadingStop;

        event EventHandler<Tag> Tag;
        event EventHandler<Tag> OpenTag;
        event EventHandler<Tag> CloseTag;
        event EventHandler<Tag> SingleTag;
        event EventHandler<Whitespace> Whitespace;
        event EventHandler<Text> Text;

        void Log(params string[] message);
        void Read();

        void Dispose();
        void ImportSettings(XmlReaderSettings settings);
        IReader CreateReaderForSubtree(Func<Tag, bool> Condition);
        IReader CreateReaderForSubtree(string elementname);

        // Legacy
        void Close();
    }
}