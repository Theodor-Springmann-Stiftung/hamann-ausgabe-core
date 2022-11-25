using System;
namespace HaDocument.Exceptions {
    public class ObjectNotFoundException : Exception {
        public ObjectNotFoundException() { }

        public ObjectNotFoundException(string msg) : base (msg) { }

        public ObjectNotFoundException(string msg, Exception inner) : base (msg, inner) { }
    }
}