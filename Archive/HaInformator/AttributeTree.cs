using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HaInformator
{
    public class AttributeTree
    {
        private Dictionary<string, Dictionary<string, HashSet<XAttribute>>> _Attributes;

        public void AddAttribute(XAttribute attribute)
        {
            if (_Attributes == null)
                _Attributes = new Dictionary<string, Dictionary<string, HashSet<XAttribute>>>();
            if (!_Attributes.ContainsKey(attribute.Name.ToString()))
                _Attributes.Add(attribute.Name.ToString(), new Dictionary<string, HashSet<XAttribute>>());
            if (!_Attributes[attribute.Name.ToString()].ContainsKey(attribute.Value))
                _Attributes[attribute.Name.ToString()].Add(attribute.Value, new HashSet<XAttribute>());
            if (!_Attributes[attribute.Name.ToString()][attribute.Value].Contains(attribute))
                _Attributes[attribute.Name.ToString()][attribute.Value].Add(attribute);
        }

        public Dictionary<string, Dictionary<string, HashSet<XAttribute>>> GetData()
                => _Attributes;

        public void AddAttribute(AttributeTree attributes)
        {
            if (attributes == null || attributes.GetData() == null)
                return;
            foreach (var aname in attributes.GetData())
                foreach (var aval in aname.Value)
                    foreach (var aindiv in aval.Value)
                        AddAttribute(aindiv);
        }

        public void RemoveAttribute(string name)
        {
            if (_Attributes == null ||
                !_Attributes.ContainsKey(name))
                return;
            foreach (var val in _Attributes[name])
                foreach (var attr in val.Value)
                    attr.Remove();
            _Attributes.Remove(name);
        }

        public void EditValue(string attr, string source, string target)
        {
            if (_Attributes == null ||
                !_Attributes.ContainsKey(attr) ||
                !_Attributes[attr].ContainsKey(source) ||
                source == target)
                return;
            foreach (var a in _Attributes[attr][source])
                a.Value = target;
        }
        
        public string[] Keys()
            => _Attributes.Keys.ToArray();

        public bool Any()
        {
            if (_Attributes == null || !_Attributes.Any())
                return false;
            return true;
        }

        public Dictionary<string, HashSet<XAttribute>> GetValue(string name)
        {
            if (_Attributes == null || !_Attributes.ContainsKey(name))
                return null;
            return _Attributes[name];
        }
    }
}
