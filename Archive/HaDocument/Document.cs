﻿using HaDocument.Models;
using HaDocument.Interfaces;
using HaDocument.Logic;
using HaDocument.Reactors;
using HaXMLReader.Interfaces;
using HaXMLReader;

namespace HaDocument
{
    /// <summary>
    /// Provides basic Access to the Letter.
    /// Initializes the parsing.
    /// Needs an Option Object as specified in IHaDocumentOptions.
    /// In case, the options change, the Object must be disposed and recreated.
    /// </summary>
    public static class Document
    {
        private static IHaDocumentOptions _settings;
        private static IReader _reader = null;
        private static IntermediateLibrary _lib = new IntermediateLibrary();
        private static ILibrary _library;

        public static ILibrary Create(IHaDocumentOptions Settings) {
            SettingsValidator.Validate(Settings);
            _settings = Settings;
            _createReader();
            _createReactors();
            _reader.Read();
            _library = _createLibrary();
            return GetLibrary();
        }

        private static void _createReactors() {
            new EditreasonReactor(_reader, _lib, _settings.NormalizeWhitespace);
            new HandDefsReactor(_reader, _lib);
            new LetterReactor(_reader, _lib, _settings.NormalizeWhitespace);
            new LocationDefsReactor(_reader, _lib);
            new MarginalReactor(_reader, _lib, _settings.NormalizeWhitespace);
            new MetaReactor(_reader, _lib, _settings.AvailableVolumes, _settings.AvailableYearRange);
            new PersonDefsReactor(_reader, _lib);
            new TraditionsReactor(_reader, _lib, _settings.NormalizeWhitespace);
            new CommentReactor(_reader, _lib, _settings.NormalizeWhitespace);
        }

        private static void _createReader() {
            _reader = new FileReader(_settings.HamannXMLFilePath);
        }

        private static ILibrary _createLibrary()
            => _lib.GetLibrary(_settings);
        
        public static ILibrary GetLibrary()
            => _library;
    }
}
