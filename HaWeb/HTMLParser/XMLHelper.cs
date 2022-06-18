namespace HaWeb.HTMLParser;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using System.Text;
using System.Collections.Generic;
using System;

public class XMLHelper<T> where T : IState
{
    protected IReader _in;
    protected StringBuilder _target;
    protected List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? _OTag_Funcs;
    protected List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? _STag_Funcs;
    protected List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? _CTag_Funcs;
    protected List<(Func<Text, XMLHelper<T>, bool>, Action<StringBuilder, Text, XMLHelper<T>>)>? _Text_Funcs;
    protected List<(Func<Whitespace, XMLHelper<T>, bool>, Action<StringBuilder, Whitespace, XMLHelper<T>>)>? _WS_Funcs;

    public T State;
    public Stack<Tag> OpenTags;
    public Dictionary<string, List<Tag>> LastSingleTags;
    public StringBuilder LastText;

    public XMLHelper(
        T state,
        IReader input,
        StringBuilder target,
        List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? OTag_Funcs = null,
        List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? STag_Funcs = null,
        List<(Func<Tag, XMLHelper<T>, bool>, Action<StringBuilder, Tag, XMLHelper<T>>)>? CTag_Funcs = null,
        List<(Func<Text, XMLHelper<T>, bool>, Action<StringBuilder, Text, XMLHelper<T>>)>? Text_Funcs = null,
        List<(Func<Whitespace, XMLHelper<T>, bool>, Action<StringBuilder, Whitespace, XMLHelper<T>>)>? WS_Funcs = null
    )
    {
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

    protected virtual void OnOTag(object? _, Tag tag)
    {
        OpenTags.Push(tag);
        if (_OTag_Funcs != null)
            foreach (var entry in _OTag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }

    protected virtual void OnText(object? _, Text text)
    {
        LastText.Append(text.Value);
        if (_Text_Funcs != null)
            foreach (var entry in _Text_Funcs)
                if (entry.Item1(text, this)) entry.Item2(_target, text, this);
    }

    protected virtual void OnSTag(object? _, Tag tag)
    {
        if (!LastSingleTags.ContainsKey(tag.Name))
            LastSingleTags.Add(tag.Name, new List<Tag>());
        LastSingleTags[tag.Name].Add(tag);
        if (_STag_Funcs != null)
            foreach (var entry in _STag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }

    protected virtual void OnCTag(object? _, Tag tag)
    {
        OpenTags.Pop();
        if(_CTag_Funcs != null)
            foreach (var entry in _CTag_Funcs)
                if (entry.Item1(tag, this)) entry.Item2(_target, tag, this);
    }

    protected virtual void OnWS(object? _, Whitespace ws)
    {
        LastText.Append(ws.Value);
        if (_WS_Funcs != null)
            foreach (var entry in _WS_Funcs)
                if (entry.Item1(ws, this)) entry.Item2(_target, ws, this);
    }

    internal void Dispose()
    {
        OpenTags.Clear();
        LastSingleTags.Clear();
        LastText.Clear();
        if (_in != null)
        {
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

    ~XMLHelper()
    {
        Dispose();
    }
}
