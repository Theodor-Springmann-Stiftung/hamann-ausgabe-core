using System;
namespace HaDocument.Exceptions {
    public class SettingsInvalidException : Exception {
        public SettingsInvalidException() { }

        public SettingsInvalidException(string msg) : base (msg) { }

        public SettingsInvalidException(string msg, Exception inner) : base (msg, inner) { }
    }
}