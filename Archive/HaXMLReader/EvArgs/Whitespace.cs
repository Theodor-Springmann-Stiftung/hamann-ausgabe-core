using HaXMLReader.Interfaces;

namespace HaXMLReader.EvArgs {
    public class Whitespace : System.EventArgs, IReaderEvArg {
        public string Value { get; set; }

    }
}