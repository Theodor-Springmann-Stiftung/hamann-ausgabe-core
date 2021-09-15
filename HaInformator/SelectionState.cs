using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaInformator
{
    public class SelectionState
    {
        public string Name { get; private set;  }
        private HashSet<Tree> _sTrees;
        private Stack<Tree> _mTrees;

        public SelectionState() { }

        public SelectionState(Tree tree)
        {
            _sTrees = new HashSet<Tree>() { tree };
            this.Name = tree != null ? tree.Name : "";
        }

        public SelectionState(HashSet<Tree> tree)
        {
            _sTrees = tree;
            this.Name = tree != null && tree.Any() ? tree.First().Name : "";
        }

        public AttributeTree GetAttributeTree()
        {
            if (_sTrees == null)
                return null;
            AttributeTree res = null;
            foreach (var t in _sTrees)
            {
                if (t.Attributes != null)
                {
                    if (res == null)
                        res = new AttributeTree();
                    res.AddAttribute(t.Attributes);
                }
            }
            return res;
        }
    
        public void Mark()
        {
            if (_sTrees == null)
                return;
            foreach (var t in _sTrees)
            {
                if (_mTrees == null)
                    _mTrees = new Stack<Tree>();
                _mTrees.Push(t);
                t.ForeColor = System.Drawing.SystemColors.HighlightText;
                t.BackColor = System.Drawing.SystemColors.Highlight;
                if (t.Parent != null)
                    _expandNodeAndParents(t);
            }
        }

        public void Unmark()
        {
            if (_mTrees == null)
                return;
            while (_mTrees.Any())
            {
                var t = _mTrees.Pop();
                t.ForeColor = Color.Black;
                t.BackColor = Color.Transparent;
            }
        }

        private void _expandNodeAndParents(Tree node)
        {
            if (_mTrees == null || node == null)
                return;
            node.Expand();
            if (node.Parent == null)
                return;
            else
                _expandNodeAndParents((Tree)node.Parent);
        }

        public void ChangeName(string name)
        {
            if (_sTrees == null || String.IsNullOrWhiteSpace(name))
                return;
            foreach (var t in _sTrees) t.SetName(name);
            this.Name = name;
        }

        public void AddElement(string name, string value = "")
        {
            if (_sTrees == null || String.IsNullOrWhiteSpace(name))
                return;
            foreach (var t in _sTrees) t.AddElement(name, value);
        }

        public void RemoveElement()
        {
            if (_sTrees == null)
                return;
            foreach (var t in _sTrees) t.RemoveElement();
        }

        public void AddAttribute(string name, string value = "")
        {
            if (_sTrees == null || String.IsNullOrWhiteSpace(name))
                return;
            foreach (var t in _sTrees) t.AddAttribute(name, value);
        }

        public void SaveTree(string filepath)
        {
            if (_sTrees == null || _sTrees.Count == 0 || String.IsNullOrWhiteSpace(filepath))
                return;
            foreach (var t in _sTrees)
                t.SaveFile(filepath);
        }

        public void EditAttribute(string source, string target)
        {
            if (_sTrees == null ||
                _sTrees.Count == 0 ||
                String.IsNullOrWhiteSpace(source) ||
                String.IsNullOrWhiteSpace(target))
                return;
            foreach (var t in _sTrees) t.EditAttribute(source, target);
        }

        public void RemoveAttribute(string target)
        {
            if (_sTrees == null ||
                _sTrees.Count == 0 ||
                String.IsNullOrWhiteSpace(target))
                return;
            foreach (var t in _sTrees) t.Attributes.RemoveAttribute(target);
        }

        public void EditValue(string attr, string source, string target)
        {
            if (_sTrees == null ||
                _sTrees.Count == 0 ||
                String.IsNullOrWhiteSpace(source))
                return;
            foreach (var t in _sTrees) t.Attributes.EditValue(attr, source, target);
        }
    }
}
