namespace HaWeb.HTMLParser;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using System.Text;
using System.Collections.Generic;
using System;


public class LineXMLHelper<T> {
    protected IReader _in;
    protected StringBuilder _target;
    protected List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? _OTag_Funcs;
    protected List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? _STag_Funcs;
    protected List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? _CTag_Funcs;
    protected List<(Func<Text, LineXMLHelper<T>, bool>, Action<StringBuilder, Text, LineXMLHelper<T>>)>? _Text_Funcs;
    protected List<(Func<Whitespace, LineXMLHelper<T>, bool>, Action<StringBuilder, Whitespace, LineXMLHelper<T>>)>? _WS_Funcs;

    public T State;
    public Stack<Tag> OpenTags;
    public Dictionary<string, List<Tag>> LastSingleTags;
    public StringBuilder LastText;

    public string CurrentPage { get; private set; }
    public string CurrentLine { get; private set; }
    public List<(string Page, string Line, string Text)>? Lines { get; private set; }
    public (string Page, string Line)? CatchPageLine { get; private set; }

    private bool _newpage;
    private bool _firstline;
    private StringBuilder _currentText;

    public LineXMLHelper(
        T state,
        IReader input,
        StringBuilder target,
        List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? OTag_Funcs = null,
        List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? STag_Funcs = null,
        List<(Func<Tag, LineXMLHelper<T>, bool>, Action<StringBuilder, Tag, LineXMLHelper<T>>)>? CTag_Funcs = null,
        List<(Func<Text, LineXMLHelper<T>, bool>, Action<StringBuilder, Text, LineXMLHelper<T>>)>? Text_Funcs = null,
        List<(Func<Whitespace, LineXMLHelper<T>, bool>, Action<StringBuilder, Whitespace, LineXMLHelper<T>>)>? WS_Funcs = null,
        string startPage = "-1",
        string startLine = "-1",
        // Set if only a specific line should be appended to the output
        (string Page, string Line)? CatchPageLine = null
    ) {

        if (input == null || target == null || state == null) throw new ArgumentNullException();

        State = state;
        _in = input;
        _target = target;

        _OTag_Funcs = OTag_Funcs;
        _STag_Funcs = STag_Funcs;
        _CTag_Funcs = CTag_Funcs;
        _Text_Funcs = Text_Funcs;
        _WS_Funcs = WS_Funcs;
        
        OpenTags = new Stack<Tag>();
        LastSingleTags = new Dictionary<string, List<Tag>>();
        LastText = new StringBuilder();

        CurrentPage = startPage;
        CurrentLine = startLine;
        _currentText = new StringBuilder();
        input.SingleTag += _setPageLine;
        input.ReadingStop += _pushLine;
        _newpage = false;
        _firstline = true;

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

    private void _pushLine(object? _, EventArgs _empty) {
        if (Lines == null) Lines = new List<(string Page, string Line, string Text)>();
        Lines.Add((CurrentPage, CurrentLine, _currentText.ToString()));
        _currentText.Clear();
    }

    private void _setPageLine(object? _, Tag tag) {
        if (tag.Name == "page" && !String.IsNullOrWhiteSpace(tag["index"])) {
            _pushLine(_, EventArgs.Empty);
            CurrentPage = tag["index"];
            _newpage = true;
        }
        if (tag.Name == "line" && !String.IsNullOrWhiteSpace(tag["index"])) {
            if (!_newpage && !_firstline) {
                _pushLine(_, EventArgs.Empty);
            }
            _firstline = false;
            _newpage = false;
            CurrentLine = tag["index"];
        }
    }

    protected virtual void OnText(object? _, Text text) {
        LastText.Append(text.Value);
        if (_Text_Funcs != null)
            if (CatchPageLine == null || (CurrentPage == CatchPageLine.Value.Page && CurrentLine == CatchPageLine.Value.Line)) {
                _currentText.Append(text.Value);
                foreach (var entry in _Text_Funcs)
                    if (entry.Item1(text, this)) entry.Item2(_target, text, this);
            }
    }

    protected virtual void OnWS(object? _, Whitespace ws) {
        LastText.Append(ws.Value);
        if (_WS_Funcs != null)
            if (CatchPageLine == null || (CurrentPage == CatchPageLine.Value.Page && CurrentLine == CatchPageLine.Value.Line)) {
                _currentText.Append(ws.Value);
                foreach (var entry in _WS_Funcs)
                    if (entry.Item1(ws, this)) entry.Item2(_target, ws, this);
            }
    }

    protected virtual void OnOTag(object? _, Tag tag) {
        OpenTags.Push(tag);
        if (_OTag_Funcs != null)
            foreach (var entry in _OTag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }


    protected virtual void OnSTag(object? _, Tag tag) {
        if (!LastSingleTags.ContainsKey(tag.Name))
            LastSingleTags.Add(tag.Name, new List<Tag>());
        LastSingleTags[tag.Name].Add(tag);
        if (_STag_Funcs != null)
            foreach (var entry in _STag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }

    protected virtual void OnCTag(object? _, Tag tag) {
        OpenTags.Pop();
        if (_CTag_Funcs != null)
            foreach (var entry in _CTag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }

    internal void Dispose() {
        OpenTags.Clear();
        LastSingleTags.Clear();
        LastText.Clear();
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

    ~LineXMLHelper() {
        Dispose();
    }
}