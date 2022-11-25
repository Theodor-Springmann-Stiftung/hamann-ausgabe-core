using System;

namespace HaDocument.Exceptions {
    public class ObjectBotchedException : Exception {
        public ObjectBotchedException() { }

        public ObjectBotchedException(string msg) : base (msg) { }

        public ObjectBotchedException(string msg, Exception inner) : base (msg, inner) { }
    }
}