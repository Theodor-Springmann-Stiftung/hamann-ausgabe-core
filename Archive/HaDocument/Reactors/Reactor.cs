using HaDocument.Models;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;

namespace HaDocument.Reactors {
    abstract class Reactor {
        protected IReader _reader;
        protected IntermediateLibrary _lib;
        protected bool _active = false;

        internal Reactor(IReader reader, IntermediateLibrary lib) {
            _reader = reader;
            _lib = lib;
        }

        protected abstract void Listen(object sender, Tag tag);
        protected abstract void Activate(IReader reader, Tag tag);
        protected abstract void Reset();
    }
}