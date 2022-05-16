namespace HaWeb.HTMLHelpers;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using System.Text;
using System.Collections.Generic;
using System;

public class XMLHelper {
    private IReader _in;
    private StringBuilder _target;
    private List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _OTag_Funcs;
    private List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _STag_Funcs;
    private List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _CTag_Funcs;
    private List<(Func<Text, bool>, Action<StringBuilder, Text>)> _Text_Funcs;
    private List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)> _WS_Funcs;
    private bool _deleteLeadingWS;
    private bool _deleteTrailingWS;

    public XMLHelper(
        IReader input,
        StringBuilder target, 
        List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> OTag_Funcs = null,
        List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> STag_Funcs = null,
        List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> CTag_Funcs = null,
        List<(Func<Text, bool>, Action<StringBuilder, Text>)> Text_Funcs = null,
        List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)> WS_Funcs = null,
        bool deleteLeadingWS = false,
        bool deleteTrailingWS = false
    ) {
        if (input == null || target == null) throw new ArgumentNullException();

        _in = input;
        _target = target;
        _deleteLeadingWS = deleteLeadingWS;
        _deleteTrailingWS = deleteTrailingWS;

        _OTag_Funcs = OTag_Funcs;
        _STag_Funcs = STag_Funcs;
        _CTag_Funcs = CTag_Funcs;
        _Text_Funcs = Text_Funcs;
        _WS_Funcs = WS_Funcs;

        if (_OTag_Funcs != null)
            _in.OpenTag += OnOTag;
        if (_STag_Funcs != null)
            _in.SingleTag += OnSTag;
        if (_CTag_Funcs != null)
            _in.CloseTag += OnCTag;
        if (_Text_Funcs != null)
            _in.Text += OnText;
        if (_WS_Funcs != null)
            _in.Whitespace += OnWS;
    }

    void OnOTag(object _, Tag tag) {
    foreach(var entry in _OTag_Funcs)
        if (entry.Item1(tag)) entry.Item2(_target, tag);
    }

    void OnText(object _, Text text) {
        if (_deleteLeadingWS) text.Value = text.Value.TrimStart();
        if (_deleteTrailingWS) text.Value = text.Value.TrimEnd();
        foreach(var entry in _Text_Funcs)
            if (entry.Item1(text)) entry.Item2(_target, text);
    }

    void OnSTag(object _, Tag tag) {
        foreach(var entry in _STag_Funcs)
            if (entry.Item1(tag)) entry.Item2(_target, tag);
    }

    void OnCTag (object _, Tag tag) {
        foreach (var entry in _CTag_Funcs)
            if (entry.Item1(tag)) entry.Item2(_target, tag);
    }

    void OnWS (object _, Whitespace ws) {
        foreach (var entry in _WS_Funcs) {
            if (entry.Item1(ws)) entry.Item2(_target, ws);
        }
    }

    internal void Dispose() {
        if (_in != null) {
            if (_OTag_Funcs != null)
                _in.OpenTag -= OnOTag;
            if (_STag_Funcs != null)
                _in.SingleTag -= OnSTag;
            if (_CTag_Funcs != null)
                _in.CloseTag -= OnCTag;
            if (_Text_Funcs != null)
                _in.Text -= OnText;
            if (_WS_Funcs != null)
                _in.Whitespace -= OnWS;
        }
    }

    ~XMLHelper() {
        Dispose();
    }
}
