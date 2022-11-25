using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace HaInformator
{
    public class Tree : TreeNode
    {
        public AttributeTree Attributes;
        public List<XElement> Elements = new List<XElement>();

        public Tree(XDocument doc) : this(doc.Root)
        {
        }

        public Tree(XElement element)
        {
            this.BeginEdit();
            this.Name = element.Name.ToString();
            this.Text = this.Name;
            this.Elements.Add(element);

            if (element.HasAttributes)
            {
                if (Attributes == null)
                    Attributes = new AttributeTree();
                foreach (var a in element.Attributes())
                    Attributes.AddAttribute(a);
            }

            if (element.HasElements)
                foreach (var e in element.Elements())
                    _addSubTree(new Tree(e));
            this.EndEdit(false);
        }

        internal void _addSubTree(Tree tree)
        {
            if (this.Nodes.ContainsKey(tree.Name))
                _mergeTreeIntoThis(tree);
            else
                this.Nodes.Add(tree);
        }

        internal void _mergeTreeIntoThis(Tree tree)
        {
            var act = (Tree)Nodes[tree.Name];
            act.Elements.AddRange(tree.Elements);
            if (tree.Attributes != null)
            {
                if (act.Attributes == null)
                    act.Attributes = new AttributeTree();
                act.Attributes.AddAttribute(tree.Attributes);
            }
            foreach (var node in tree.Nodes)
                act._addSubTree((Tree)node);
        }

        public void SetName(string name)
        {
            this.BeginEdit();
            foreach (var elem in Elements)
                elem.Name = name;
            if (Parent == null || !Parent.Nodes.ContainsKey(name))
            {
                this.Name = name;
                this.Text = name;
                this.EndEdit(false);
                return;
            }
            else
            {
                this.Name = name;
                this.Text = name;
                var p = (Tree)Parent;
                this.Remove();
                p._addSubTree(this);
                this.EndEdit(false);
            }
        }

        public void AddElement(string name, string value)
        {
            this.BeginEdit();
            foreach (var elem in Elements)
            {
                var e = new XElement(name);
                if (!String.IsNullOrWhiteSpace(value))
                    e.Value = value;
                elem.Add(e);
                _addSubTree(new Tree(e));
            }
            this.EndEdit(false);
            return;
        }

        public void RemoveElement()
        {
            this.BeginEdit();
            foreach (var elem in Elements)
                elem.Remove();
            this.Remove();
            this.EndEdit(false);
        }

        public void AddAttribute(string name, string value)
        {
            this.BeginEdit();
            foreach (var elem in Elements)
            {
                var a = new XAttribute(name, value);
                if (!String.IsNullOrWhiteSpace(value))
                    a.Value = value;
                if (!elem.Attributes().Where(x => x.Name == name).Any())
                    elem.Add(a);
                if (Attributes == null)
                    Attributes = new AttributeTree();
                Attributes.AddAttribute(a);
            }
            // Why was this here?
            //if (Parent == null)
            //    return;
            //var p = (Tree)Parent;
            //this.Remove();
            //p._addSubTree(this);
            this.EndEdit(false);
        }
        public void EditAttribute(string source, string target)
        {
            if (source == target)
                return;
            foreach (var elem in Elements)
                if (elem.HasAttributes)
                    if (elem.Attributes(source).Any())
                        foreach (var a in elem.Attributes(source))
                        {
                            var attr = new XAttribute(target, a.Value);
                            elem.Add(attr);
                            a.Remove();
                            Attributes.AddAttribute(attr);
                        }
            Attributes.RemoveAttribute(source);
        }

        public void SaveFile(string filepath)
        {
            foreach (var elem in Elements)
            {
                var sb = new System.IO.FileStream(filepath, System.IO.FileMode.Create);
                var set = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using (var writer = XmlWriter.Create(sb, set))
                {
                    elem.Save(writer);
                }
                sb.Close();
            }
        }
    }


}
