using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaInformator
{
    public class FlatCollection
    {
        private Dictionary<string, HashSet<Tree>> _FlatCollection;

        public FlatCollection(Tree tree)
        {
            _addToCollection(tree);
        }

        public FlatCollection(TreeNodeCollection coll)
        {
            foreach (var t in coll) _addToCollection((Tree)t);
        } 

        private void _addToCollection(Tree tree)
        {
            if (_FlatCollection == null)
                _FlatCollection = new Dictionary<string, HashSet<Tree>>();
            if (!_FlatCollection.ContainsKey(tree.Name))
                _FlatCollection.Add(tree.Name, new HashSet<Tree>());
            if (!_FlatCollection[tree.Name].Contains(tree))
                _FlatCollection[tree.Name].Add(tree);

            foreach (var t in tree.Nodes)
                _addToCollection((Tree)t);
        }

        public string[] Keys()
        {
            if (_FlatCollection == null)
                return null;
            return _FlatCollection.Keys.ToArray();
        }

        public HashSet<Tree> GetTrees(string key)
        {
            if (!_FlatCollection.ContainsKey(key))
                return null;
            else
                return _FlatCollection[key];
        }
    }
}
